using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TimelessTechnicians.UI.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using static TimelessTechnicians.UI.Models.ApplicationUser;
using TimelessTechnicians.UI.ViewModel;
using TimelessTechnicians.UI.Services;
using Microsoft.AspNetCore.Authorization;
using System.Numerics;
using TimelessTechnicians.UI.Data;
using System.ComponentModel.DataAnnotations;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout;


namespace TimelessTechnicians.UI.Controllers
{

    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuditLogger _auditLogger;

        public AccountController(ApplicationDbContext context,  IAuditLogger auditLogger)
        {
            _context = context;
            _auditLogger = auditLogger;
        }
        public IActionResult Register()
        {
            PopulateViewBag();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(ApplicationUser register)
        {
            if (ModelState.IsValid)
            {
                if (!register.TermsOfServiceAccepted)
                {
                    TempData["ErrorMessage"] = "You must accept the terms of service.";
                    TempData["MessageType"] = "error";
                    return View(register);
                }

                if (register.DateOfBirth > DateTime.Now)
                {
                    TempData["ErrorMessage"] = "Date of Birth cannot be in the future.";
                    TempData["MessageType"] = "error";
                    return View(register);
                }

                var passwordHasher = new PasswordHasher<ApplicationUser>();
                register.PasswordHash = passwordHasher.HashPassword(register, register.Password);
                register.Status = (register.Role == ApplicationUser.UserRole.PATIENT) ? UserStatus.Inactive : UserStatus.Active;

                // Add the user to the context
                _context.Add(register);
                await _context.SaveChangesAsync();

                // Log the registration action
                await _auditLogger.LogAsync(
                    action: "User Registered",
                    firstName: register.FirstName,
                    lastName: register.LastName,
                    details: $"User {register.UserName} registered successfully."
                );

                TempData["SuccessMessage"] = $"{register.FirstName} {register.LastName}, registration successful. You can now log in.";
                TempData["MessageType"] = "success";

                return RedirectToAction(nameof(Login));
            }

            // If we reach this point, something went wrong; repopulate the ViewBag
            PopulateViewBag(register);
            TempData["ErrorMessage"] = "Please correct the errors in the form.";
            TempData["MessageType"] = "error";

            return View(register);
        }



        public IActionResult Login()
        {
            // Check if "UserEmail" cookie exists
            if (Request.Cookies.ContainsKey("UserEmail"))
            {
                // Retrieve the email from the cookie
                string rememberedEmail = Request.Cookies["UserEmail"];

                // Pre-fill the model with the remembered email
                var model = new Login
                {
                    UserNameorEmail = rememberedEmail
                };

                return View(model);
            }

            // Return the view without pre-filling if no cookie exists
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Login model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Register
                    .FirstOrDefaultAsync(u => u.UserName == model.UserNameorEmail || u.Email == model.UserNameorEmail);

                if (user != null)
                {
                    // Check if the user's status is not Active
                    if (user.Status != UserStatus.Active)
                    {
                        TempData["ErrorMessage"] = "Your account is inactive. Please contact support.";
                        await _auditLogger.LogAsync("Login Failed", user.FirstName, user.LastName, "Attempt to log in with an inactive account.");
                        return View(model);
                    }

                    if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.UtcNow)
                    {
                        var remainingLockoutTime = user.LockoutEnd.Value - DateTimeOffset.UtcNow;
                        TempData["ErrorMessage"] = "Your account is locked due to multiple failed login attempts.";
                        await _auditLogger.LogAsync("Login Failed", user.FirstName, user.LastName, "Account is locked out.");
                        ViewBag.RemainingLockoutTime = remainingLockoutTime.TotalSeconds;
                        return View(model);
                    }

                    var passwordHasher = new PasswordHasher<ApplicationUser>();
                    var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);

                    if (result == PasswordVerificationResult.Success)
                    {
                        // Reset access failed count and lockout
                        user.AccessFailedCount = 0;
                        user.LockoutEnd = null;

                        // Create claims
                        var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role.ToString()),
                    
                };

                        if (user.Role == UserRole.DOCTOR)
                        {
                            claims.Add(new Claim("DoctorId", user.Id.ToString()));
                            claims.Add(new Claim("FirstName", user.FirstName)); // Add First Name claim
                            claims.Add(new Claim("LastName", user.LastName));   // Add Last Name claim
                        }


                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = model.RememberMe
                        };

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                        // Manage "Remember Me" functionality
                        if (model.RememberMe)
                        {
                            Response.Cookies.Append("UserEmail", model.UserNameorEmail, new CookieOptions
                            {
                                Expires = DateTimeOffset.UtcNow.AddDays(30),
                                HttpOnly = true,
                                Secure = Request.IsHttps
                            });
                        }
                        else
                        {
                            Response.Cookies.Delete("UserEmail");
                        }

                        await _auditLogger.LogAsync("Login Success", user.FirstName, user.LastName, "User logged in successfully.");
                        TempData["SuccessMessage"] = $"Login successful! Welcome back, {user.FirstName} {user.LastName}.";
                        return RedirectToActionBasedOnRole(user.Role.ToString());
                    }
                    else
                    {
                        user.AccessFailedCount++;

                        if (user.AccessFailedCount >= 5)
                        {
                            user.LockoutEnd = DateTimeOffset.UtcNow.AddMinutes(30);
                            user.Status = UserStatus.Suspended;
                            await _auditLogger.LogAsync("Account Locked", user.FirstName, user.LastName, "Account locked due to multiple failed login attempts.");
                        }

                        await _context.SaveChangesAsync();
                        TempData["ErrorMessage"] = "Invalid username/email or password.";
                        await _auditLogger.LogAsync("Login Failed", user.FirstName, user.LastName, "Invalid username/email or password.");
                    }
                }
                else
                {
                    await _auditLogger.LogAsync("Login Failed", "Unknown", "Unknown", "Invalid username/email or password.");
                    TempData["ErrorMessage"] = "Invalid username/email or password.";
                }
            }

            return View(model);
        }


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            TempData["SuccessMessage"] = "You have been successfully logged out.";
            TempData["MessageType"] = "success";

            return RedirectToAction(nameof(Login));
        }


       

        public IActionResult ChangePassword(string email)
        {
            var model = new ChangePasswordViewModel { Email = email };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Register.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (user != null)
                {
                    var passwordHasher = new PasswordHasher<ApplicationUser>();
                    var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.CurrentPassword);

                    if (result == PasswordVerificationResult.Success)
                    {
                        if (model.NewPassword == model.ConfirmPassword)
                        {
                            user.PasswordHash = passwordHasher.HashPassword(user, model.NewPassword);
                            await _context.SaveChangesAsync();

                            TempData["SuccessMessage"] = $"{user.FirstName} {user.LastName}, your password has been changed successfully.";
                            TempData["MessageType"] = "success";
                            return RedirectToAction(nameof(Login));
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "The new password and confirmation password do not match.";
                            TempData["MessageType"] = "error";
                        }
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "The current password is incorrect.";
                        TempData["MessageType"] = "error";
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "User not found.";
                    TempData["MessageType"] = "error";
                }
            }

            // Optionally, repopulate any necessary ViewBag or ModelState errors here
            return View(model);
        }


        public IActionResult ChangeEmail(string email)
        {
            var model = new ChangeEmailViewModel { CurrentEmail = email };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeEmail(ChangeEmailViewModel model)
        {
            if (ModelState.IsValid)
            {
                var currentEmailLower = model.CurrentEmail.ToLower();
                var newEmailLower = model.NewEmail.ToLower();

                var user = await _context.Register.FirstOrDefaultAsync(u => u.Email.ToLower() == currentEmailLower);

                if (user != null)
                {
                    if (await _context.Register.AnyAsync(u => u.Email.ToLower() == newEmailLower))
                    {
                        TempData["ErrorMessage"] = "The new email address is already in use.";
                        TempData["MessageType"] = "error";

                        // Log the attempt to change to an already used email
                        await _auditLogger.LogAsync(
                            action: "Change Email Attempt Failed",
                            firstName: user.FirstName,
                            lastName: user.LastName,
                            details: $"Attempted to change email from {currentEmailLower} to {newEmailLower}, but the new email is already in use."
                        );
                    }
                    else
                    {
                        // Log the successful email change
                        await _auditLogger.LogAsync(
                            action: "Email Changed",
                            firstName: user.FirstName,
                            lastName: user.LastName,
                            details: $"Email changed from {currentEmailLower} to {newEmailLower}."
                        );

                        user.Email = model.NewEmail;
                        await _context.SaveChangesAsync();

                        TempData["SuccessMessage"] = $"{user.FirstName} {user.LastName}, your email has been changed successfully.";
                        TempData["MessageType"] = "success";
                        return RedirectToAction(nameof(Login));
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "User not found.";
                    TempData["MessageType"] = "error";
                }
            }

            return View(model);
        }




        [Authorize(Roles = "ADMINISTRATOR")]
        public async Task<IActionResult> UserManagement(string searchQuery, string statusFilter, string sortOrder, int pageNumber = 1, int pageSize = 10)
        {
            ViewData["ShowSidebar"] = true;

            // Fetch all users
            var usersQuery = _context.Register.AsQueryable();

            // Apply search filtering
            if (!string.IsNullOrEmpty(searchQuery))
            {
                usersQuery = usersQuery.Where(u =>
                    u.UserName.Contains(searchQuery) ||
                    u.Email.Contains(searchQuery) ||
                    u.FirstName.Contains(searchQuery) ||
                    u.LastName.Contains(searchQuery));
            }

            // Apply status filtering
            if (!string.IsNullOrEmpty(statusFilter))
            {
                if (Enum.TryParse<UserStatus>(statusFilter, out var status))
                {
                    usersQuery = usersQuery.Where(u => u.Status == status);
                }
            }

            // Apply sorting
            usersQuery = sortOrder switch
            {
                "username_desc" => usersQuery.OrderByDescending(u => u.UserName),
                "email" => usersQuery.OrderBy(u => u.Email),
                "email_desc" => usersQuery.OrderByDescending(u => u.Email),
                "status" => usersQuery.OrderBy(u => u.Status),
                "status_desc" => usersQuery.OrderByDescending(u => u.Status),
                _ => usersQuery.OrderBy(u => u.UserName),
            };

            // Pagination
            var totalUsers = await usersQuery.CountAsync();
            var users = await usersQuery.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            // Pass pagination info to view
            ViewData["CurrentSort"] = sortOrder;
            ViewData["PageNumber"] = pageNumber;
            ViewData["TotalPages"] = (int)Math.Ceiling(totalUsers / (double)pageSize);

            return View(users);
        }





        [Authorize(Roles = "ADMINISTRATOR")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("UnlockAccountPost")]
        public async Task<IActionResult> UnlockAccountPost(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    TempData["ErrorMessage"] = "Invalid user ID.";
                    return RedirectToAction("UserManagement");
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "User not found.";
                    return RedirectToAction("UserManagement");
                }

                // Unlock logic...
                user.LockoutEnd = null;
                await _context.SaveChangesAsync();

                // Log the successful unlocking of the account
                await _auditLogger.LogAsync(
                    action: "Account Unlocked",
                    firstName: user.FirstName,
                    lastName: user.LastName,
                    details: $"Account with ID {userId} was unlocked."
                );

                TempData["SuccessMessage"] = $"{user.FirstName} {user.LastName}, your account has been unlocked successfully.";
                return RedirectToAction("UserManagement");
            }
            catch (Exception ex)
            {
                // Log the exception
                await _auditLogger.LogAsync("Unlock Account Failed", "System", "Admin", ex.Message);
                TempData["ErrorMessage"] = "An error occurred while unlocking the account. Please try again.";
                return RedirectToAction("UserManagement");
            }
        }


        public async Task<IActionResult> UpdateProfile()
        {
            ViewData["ShowSidebar"] = true;

            var user = await _context.Register.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }

            var role = user.Role.ToString();

            var redirectUrl = GetRedirectUrlBasedOnRole(role);

            var model = new UserProfileViewModel
            {
                Title = user.Title,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                DateOfBirth = user.DateOfBirth,
                City = user.City,
                Suburb = user.Suburb,
                Address = user.Address
            };

            ViewBag.CancelRedirectUrl = redirectUrl;

            return View(model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(UserProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Register.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "User not found.";
                    TempData["MessageType"] = "error";
                    return RedirectToAction(nameof(UpdateProfile));
                }

                // Log previous user details for audit before the update
                string previousDetails = $"Previous: Title: {user.Title}, Name: {user.FirstName} {user.LastName}, Date of Birth: {user.DateOfBirth}, City: {user.City}, Suburb: {user.Suburb}, Address: {user.Address}";

                // Update user logic
                user.Title = model.Title;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.DateOfBirth = model.DateOfBirth;
                user.City = model.City;
                user.Suburb = model.Suburb;
                user.Address = model.Address;

                await _context.SaveChangesAsync();

                // Log the successful profile update
                await _auditLogger.LogAsync(
                    action: "Profile Updated",
                    firstName: user.FirstName,
                    lastName: user.LastName,
                    details: $"{previousDetails}. Updated to: Title: {model.Title}, Name: {model.FirstName} {model.LastName}, Date of Birth: {model.DateOfBirth}, City: {model.City}, Suburb: {model.Suburb}, Address: {model.Address}"
                );

                TempData["SuccessMessage"] = $"{user.FirstName} {user.LastName}, your profile has been updated successfully.";
                TempData["MessageType"] = "success";

                return RedirectToAction(nameof(RedirectToDashboard), new { role = user.Role.ToString() });
            }

            return View(model);
        }


    
        public async Task<IActionResult> DeactivateProfile()
        {

            ViewData["ShowSidebar"] = true;
            var user = await _context.Register.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }

            var model = new UserProfileViewModel
            {
                Title = user.Title,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                DateOfBirth = user.DateOfBirth,
                City = user.City,
                Suburb = user.Suburb,
                Address = user.Address
            };

            ViewBag.CancelRedirectUrl = GetRedirectUrlBasedOnRole(user.Role.ToString());

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateProfileConfirmed()
        {
            var user = await _context.Register.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                TempData["MessageType"] = "error";
                return RedirectToAction(nameof(DeactivateProfile));
            }

            // Log the user's details before deactivation
            string previousStatus = user.Status.ToString(); // Store the previous status

            // Deactivate the user profile
            user.Status = UserStatus.Inactive;
            await _context.SaveChangesAsync();

            // Log the deactivation action
            await _auditLogger.LogAsync(
                action: "Profile Deactivated",
                firstName: user.FirstName,
                lastName: user.LastName,
                details: $"User status changed from {previousStatus} to {user.Status}. Profile deactivated by {User.Identity.Name}."
            );

            TempData["SuccessMessage"] = $"{user.FirstName} {user.LastName}, your profile has been deactivated.";
            TempData["MessageType"] = "success";

            return RedirectToAction("Login", "Account");
        }




        [AllowAnonymous]
        public async Task<IActionResult> AccessDenied()
        {
            ViewData["ShowSidebar"] = true;
            ViewBag.ErrorMessage = "You do not have the necessary permissions to view this page.";

            // Determine the user's role and set the return URL
            string returnUrl = Url.Action("Index", "Home"); // Default return URL
            string roleSpecificMessage = "You do not have permission to access this page.";


            ViewBag.ReturnUrl = returnUrl;
            ViewBag.RoleSpecificMessage = roleSpecificMessage;
          

            return View();
        }


        [Authorize(Roles = "ADMINISTRATOR")]
        public async Task<IActionResult> ViewLogs(int page = 1, int pageSize = 20)
        {
            ViewData["ShowSidebar"] = true;

            // Fetch total log count
            var totalLogs = await _context.AuditLogs.CountAsync();

            // Fetch paginated logs
            var logs = await _context.AuditLogs
                .OrderByDescending(l => l.ActionDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Prepare ViewModel with pagination info
            var viewModel = new LogViewModel
            {
                Logs = logs,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalLogs / (double)pageSize)
            };

            return View(viewModel);
        }
        [Authorize(Roles = "ADMINISTRATOR")]
        [HttpGet]
        public async Task<IActionResult> ReactivateAccount(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "Invalid user ID.";
                TempData["MessageType"] = "error";
                return RedirectToAction("UserManagement");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                TempData["MessageType"] = "error";
                return RedirectToAction("UserManagement");
            }

            user.Status = UserStatus.Active;
            user.LockoutEnd = null;
            user.AccessFailedCount = 0;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            await _auditLogger.LogAsync("Account Reactivated", user.FirstName, user.LastName, $"Account with ID {userId} was reactivated.");

            TempData["SuccessMessage"] = $"{user.FirstName} {user.LastName}, your account has been reactivated successfully.";
            TempData["MessageType"] = "success";
            return RedirectToAction("UserManagement");
        }






        [HttpPost]
        [Authorize(Roles = "ADMINISTRATOR")]
        public async Task<IActionResult> BulkAction(string action, string[] selectedUserIds)
        {
            if (selectedUserIds == null || !selectedUserIds.Any())
            {
                // No users selected
                TempData["ErrorMessage"] = "No users were selected for the bulk action.";
                return RedirectToAction(nameof(UserManagement));
            }

            var users = await _context.Users.Where(u => selectedUserIds.Contains(u.Id)).ToListAsync();

            switch (action)
            {
                case "unlock":
                    foreach (var user in users)
                    {
                        if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.UtcNow)
                        {
                            user.LockoutEnd = null;
                            user.AccessFailedCount = 0;
                        }
                    }
                    TempData["SuccessMessage"] = "Selected users have been unlocked.";
                    break;

                case "deactivate":
                    foreach (var user in users)
                    {
                        if (user.Status == UserStatus.Active)
                        {
                            user.Status = UserStatus.Inactive;
                        }
                    }
                    TempData["SuccessMessage"] = "Selected users have been deactivated.";
                    break;

                case "delete":
                    _context.Users.RemoveRange(users);
                    TempData["SuccessMessage"] = "Selected users have been deleted.";
                    break;

                case "reactivate":
                    foreach (var user in users)
                    {
                        if (user.Status == UserStatus.Inactive)
                        {
                            user.Status = UserStatus.Active; // Set status to Active
                            user.LockoutEnd = null; // Reset lockout end time if applicable
                            user.AccessFailedCount = 0; // Reset failed access count
                        }
                    }
                    TempData["SuccessMessage"] = "Selected users have been reactivated.";
                    break;

                default:
                    TempData["ErrorMessage"] = "Invalid action.";
                    break;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(UserManagement));
        }















































        private bool IsInvalidPassword(string password, string firstName, string lastName, DateTime dateOfBirth, IEnumerable<UserRole> userRoles, string email, string username)
        {
            // Length requirements
            const int minLength = 8; // Minimum length for the password
            const int maxLength = 20; // Maximum length for the password

            // Check minimum length
            if (password.Length < minLength)
            {
                return true; // Invalid due to being too short
            }

            // Check maximum length
            if (password.Length > maxLength)
            {
                return true; // Invalid due to being too long
            }

            // Disallow common passwords
            var commonPasswords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "12345678", "password", "123456", "123456789", "qwerty", "abc123", "letmein", "welcome", "monkey", "password1"
        // Add more common passwords as needed
    };

            if (commonPasswords.Contains(password))
            {
                return true; // Invalid due to being a common password
            }

            // Disallow password as a combination of first name + last name (case insensitive)
            var nameCombination = $"{firstName}{lastName}";
            if (password.Contains(nameCombination, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            // Disallow password matching DateOfBirth
            if (DateTime.TryParse(password, out DateTime passwordDate) && passwordDate == dateOfBirth)
            {
                return true;
            }

            // Disallow password being the same as any UserRole (case insensitive)
            if (userRoles.Any(role => password.Equals(role.ToString(), StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }

            // Avoid personal information (email, username)
            if (ContainsPersonalInformation(password, email, username))
            {
                return true; // Invalid due to containing personal information
            }

            // Complexity requirements
            bool hasUpperCase = password.Any(char.IsUpper); // At least one uppercase letter
            bool hasLowerCase = password.Any(char.IsLower); // At least one lowercase letter
            bool hasDigit = password.Any(char.IsDigit); // At least one digit
            bool hasSpecialChar = password.Any(c => "!@#$%^&*()_+-=[]{}|;:'\",.<>?/`~".Contains(c)); // At least one special character

            if (!hasUpperCase || !hasLowerCase || !hasDigit || !hasSpecialChar)
            {
                return true; // Invalid due to failing complexity requirements
            }

            // Check for sequential characters
            if (HasSequentialCharacters(password))
            {
                return true; // Invalid due to containing sequential characters
            }

            // Check for repetitive characters
            if (HasRepetitiveCharacters(password))
            {
                return true; // Invalid due to containing repetitive characters
            }



            return false; // The password is valid
        }

        // Method to check if the password contains personal information
        private bool ContainsPersonalInformation(string password, string email, string username)
        {
            // Convert email and username to lowercase for case-insensitive comparison
            var lowerEmail = email.ToLower();
            var lowerUsername = username.ToLower();

            // Check for email parts in the password
            if (password.Contains(lowerEmail) || password.Contains(lowerEmail.Split('@')[0])) // Before the '@' symbol
            {
                return true; // Password contains part of the email
            }

            // Check for username in the password
            if (password.Contains(lowerUsername))
            {
                return true; // Password contains the username
            }

            return false; // No personal information found in the password
        }

        // Method to check for sequential characters
        private bool HasSequentialCharacters(string password)
        {
            string[] sequences = { "abc", "123", "qwerty", "asdf", "zxcv", "ijkl" }; // Add more sequences as needed

            foreach (var seq in sequences)
            {
                if (password.IndexOf(seq, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true; // Sequential characters found
                }
            }

            return false; // No sequential characters found
        }

        // Method to check for repetitive characters
        private bool HasRepetitiveCharacters(string password)
        {
            for (int i = 0; i < password.Length - 2; i++)
            {
                // Check for three consecutive identical characters
                if (password[i] == password[i + 1] && password[i] == password[i + 2])
                {
                    return true; // Repetitive characters found
                }
            }

            return false; // No repetitive characters found
        }


        // Redirects based on the user's role
        private IActionResult RedirectToActionBasedOnRole(string role)
        {
            switch (role)
            {
                case nameof(UserRole.ADMINISTRATOR):
                    return RedirectToAction("AdminDashboard", "Admin");
                case nameof(UserRole.WARDADMIN):
                    return RedirectToAction("WardAdminDashboard", "WardAdmin");
                case nameof(UserRole.PATIENT):
                    return RedirectToAction("PatientDashboard", "Patient");
                case nameof(UserRole.NURSE):
                    return RedirectToAction("NurseDashboard", "Nurse");
                case nameof(UserRole.NURSINGSISTER):
                    return RedirectToAction("NursingSisterDashboard", "NursingSister");
                case nameof(UserRole.DOCTOR):
                    return RedirectToAction("DoctorDashboard", "Doctor");
                case nameof(UserRole.SCRIPTMANAGER):
                    return RedirectToAction("ScriptManagerDashboard", "ScriptManager");
                case nameof(UserRole.CONSUMABLESMANAGER):
                    return RedirectToAction("ConsumablesManagerDashboard", "ConsumablesManager");
                default:
                    return RedirectToAction("Index", "Home");
            }
        }


        public IActionResult RedirectToDashboard(string role)
        {
            switch (role)
            {
                case nameof(UserRole.ADMINISTRATOR):
                    return RedirectToAction("AdminDashboard", "Admin");
                case nameof(UserRole.WARDADMIN):
                    return RedirectToAction("WardAdminDashboard", "WardAdmin");
                case nameof(UserRole.PATIENT):
                    return RedirectToAction("PatientDashboard", "Patient");
                case nameof(UserRole.NURSE):
                    return RedirectToAction("NurseDashboard", "Nurse");
                case nameof(UserRole.NURSINGSISTER):
                    return RedirectToAction("NursingSisterDashboard", "NursingSister");
                case nameof(UserRole.DOCTOR):
                    return RedirectToAction("DoctorDashboard", "Doctor");
                case nameof(UserRole.SCRIPTMANAGER):
                    return RedirectToAction("ScriptManagerDashboard", "ScriptManager");
                case nameof(UserRole.CONSUMABLESMANAGER):
                    return RedirectToAction("ConsumablesManagerDashboard", "ConsumablesManager");
                default:
                    return RedirectToAction("Index", "Home");
            }
        }

        private string GetRedirectUrlBasedOnRole(string role)
        {
            switch (role)
            {
                case nameof(UserRole.ADMINISTRATOR):
                    return Url.Action("AdminDashboard", "Admin");
                case nameof(UserRole.WARDADMIN):
                    return Url.Action("WardAdminDashboard", "WardAdmin");
                case nameof(UserRole.PATIENT):
                    return Url.Action("PatientDashboard", "Patient");
                case nameof(UserRole.NURSE):
                    return Url.Action("NurseDashboard", "Nurse");
                case nameof(UserRole.NURSINGSISTER):
                    return Url.Action("NursingSisterDashboard", "NursingSister");
                case nameof(UserRole.DOCTOR):
                    return Url.Action("DoctorDashboard", "Doctor");
                case nameof(UserRole.SCRIPTMANAGER):
                    return Url.Action("ScriptManagerDashboard", "ScriptManager");
                case nameof(UserRole.CONSUMABLESMANAGER):
                    return Url.Action("ConsumablesManagerDashboard", "ConsumablesManager");
                default:
                    return Url.Action("Index", "Home");
            }
        }

        private void PopulateViewBag(ApplicationUser register = null)
        {
            // Sort titles alphabetically
            ViewBag.TitleList = new SelectList(Enum.GetValues(typeof(Titles))
                                               .Cast<Titles>()
                                               .OrderBy(t => t.ToString()), register?.Title);

            // Sort cities alphabetically
            ViewBag.CityList = new SelectList(Enum.GetValues(typeof(Cities))
                                              .Cast<Cities>()
                                              .OrderBy(c => c.ToString()), register?.City);

            // Sort suburbs alphabetically
            ViewBag.SuburbList = new SelectList(Enum.GetValues(typeof(Suburbs))
                                                .Cast<Suburbs>()
                                                .OrderBy(s => s.ToString()), register?.Suburb);

            // Sort roles alphabetically
            ViewBag.RoleList = new SelectList(Enum.GetValues(typeof(UserRole))
                                              .Cast<UserRole>()
                                              .OrderBy(r => r.ToString()), register?.Role);

            // Sort user statuses alphabetically
            ViewBag.UserStatusList = new SelectList(Enum.GetValues(typeof(UserStatus))
                                                    .Cast<UserStatus>()
                                                    .OrderBy(us => us.ToString()), register?.Status);
        }



    }
}
