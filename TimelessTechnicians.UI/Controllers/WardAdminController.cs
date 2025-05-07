using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using TimelessTechnicians.UI.Data;
using TimelessTechnicians.UI.Models;
using TimelessTechnicians.UI.Services;
using TimelessTechnicians.UI.ViewModel;
using TimelessTechnicians.UI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;
using static TimelessTechnicians.UI.Models.ApplicationUser;
using X.PagedList.Extensions;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;

namespace TimelessTechnicians.UI.Controllers
{
    [Authorize(Roles = "WARDADMIN,DOCTOR")]
    public class WardAdminController : Controller
    {

        private readonly ApplicationDbContext _context;

        public WardAdminController(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> WardAdminDashboard()
        {
            ViewData["ShowSidebar"] = true;

            // Fetch all patients
            var allPatients = _context.Register
                .Where(u => u.Role == ApplicationUser.UserRole.PATIENT);

            // Count confirmed and pending patients
            int confirmedPatients = await allPatients.CountAsync(p => p.Status == UserStatus.Active);
            int pendingConfirmations = await allPatients.CountAsync(p => p.Status == UserStatus.Inactive);

            // Count patients with folders
            int patientsWithFolders = await _context.PatientFolders.CountAsync();

            // Count total discharges
            int totalDischarges = await _context.PatientDischargeInstructions
                .Where(pd => pd.Status != InstructionStatus.Deleted)
                .CountAsync();

            // Count bed statistics
            int totalBeds = await _context.Beds.CountAsync();
            int occupiedBeds = await _context.Beds.CountAsync(b => b.Status == BedStatus.Occupied);
            int availableBeds = totalBeds - occupiedBeds;

            // Get count of occupied beds by ward
            var bedsByWard = await _context.Beds
                .Where(b => b.Status == BedStatus.Occupied)
                .GroupBy(b => b.Ward.WardName)
                .Select(g => new { WardName = g.Key, Count = g.Count() })
                .ToListAsync();

            // Create the view model
            var model = new WardAdminDashboardViewModel
            {
                TotalPatients = await allPatients.CountAsync(),
                ConfirmedPatients = confirmedPatients,
                PendingConfirmations = pendingConfirmations,
                PatientsWithFolder = patientsWithFolders,
                TotalDischarges = totalDischarges,
                TotalBeds = totalBeds,
                OccupiedBeds = occupiedBeds,
                AvailableBeds = availableBeds,
                BedsByWard = bedsByWard.ToDictionary(b => b.WardName, b => b.Count)
            };

            // Return the view with the model
            return View(model);
        }









        public async Task<IActionResult> ListPatients(string sortOrder, string searchString, int? page)
         {
        ViewData["ShowSidebar"] = true; // Show the sidebar

        // Fetch all users with the role of "Patient" and an Active status
        var patients = _context.Register
            .Where(u => u.Role == ApplicationUser.UserRole.PATIENT && u.Status == UserStatus.Active);

        // Apply search filter if a search term is provided
        if (!string.IsNullOrEmpty(searchString))
        {
            searchString = searchString.ToLower(); // Normalize search term
            patients = patients.Where(u => u.FirstName.ToLower().Contains(searchString) ||
                                            u.LastName.ToLower().Contains(searchString) ||
                                            u.Email.ToLower().Contains(searchString) ||
                                            u.Address.ToLower().Contains(searchString));
        }

        // Sort patients based on the sortOrder parameter
        switch (sortOrder)
        {
            case "firstName_desc":
                patients = patients.OrderByDescending(u => u.FirstName);
                break;
            case "lastName":
                patients = patients.OrderBy(u => u.LastName);
                break;
            case "lastName_desc":
                patients = patients.OrderByDescending(u => u.LastName);
                break;
            case "dateOfBirth":
                patients = patients.OrderBy(u => u.DateOfBirth);
                break;
            case "dateOfBirth_desc":
                patients = patients.OrderByDescending(u => u.DateOfBirth);
                break;
            default: // First name ascending by default
                patients = patients.OrderBy(u => u.FirstName);
                break;
        }

        // Set the page size
        int pageSize = 10; // You can adjust this to your needs
        int pageNumber = page ?? 1; // Default to first page if no page is specified

        // Get the paginated list of active patients (synchronous method)
        var activePatients = patients.ToPagedList(pageNumber, pageSize);

        // Pass the current search string to the ViewData for retaining its value in the view
        ViewData["CurrentFilter"] = searchString;

        return View(activePatients);
    }



        



        public async Task<IActionResult> ConfirmPatients()
        {
            ViewData["ShowSidebar"] = true;

            // Fetch all users with the role of "Patient"
            var patients = await _context.Users
                .Where(u => u.Role == ApplicationUser.UserRole.PATIENT && u.Status == UserStatus.Inactive)
                .ToListAsync();

            return View(patients);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmPatient(string id)
        {
            var patient = await _context.Users.FindAsync(id);
            if (patient == null)
            {
                TempData["ErrorMessage"] = "Patient not found.";
                return RedirectToAction(nameof(ConfirmPatients));
            }

            // Update patient status to Active
            patient.Status = UserStatus.Active;

            // Save changes to the database
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Patient confirmed successfully.";
            return RedirectToAction(nameof(ListPatients));
        }



        public async Task<IActionResult> ExportPatients(string sortOrder, string searchString)
        {
            var patients = _context.Register
       .Where(u => u.Role == ApplicationUser.UserRole.PATIENT && u.Status == UserStatus.Active);

            // Apply search filter
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower(); // Normalize search term
                patients = patients.Where(u => u.FirstName.ToLower().Contains(searchString) ||
                                                u.LastName.ToLower().Contains(searchString) ||
                                                u.Email.ToLower().Contains(searchString) ||
                                                u.Address.ToLower().Contains(searchString));
            }

            // Sort patients
            switch (sortOrder)
            {
                case "firstName_desc":
                    patients = patients.OrderByDescending(u => u.FirstName);
                    break;
                case "lastName":
                    patients = patients.OrderBy(u => u.LastName);
                    break;
                case "lastName_desc":
                    patients = patients.OrderByDescending(u => u.LastName);
                    break;
                case "dateOfBirth":
                    patients = patients.OrderBy(u => u.DateOfBirth);
                    break;
                case "dateOfBirth_desc":
                    patients = patients.OrderByDescending(u => u.DateOfBirth);
                    break;
                default: // First name ascending by default
                    patients = patients.OrderBy(u => u.FirstName);
                    break;
            }

            // Generate the PDF
            using (var memoryStream = new MemoryStream())
            {
                // Initialize PDF writer and document
                var writer = new PdfWriter(memoryStream);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf);

                // Add title
                document.Add(new Paragraph("Patient List")
                    .SetFontSize(20)
                    .SetBold());

                // Create table
                var table = new Table(6);
                table.AddHeaderCell("Title");
                table.AddHeaderCell("First Name");
                table.AddHeaderCell("Last Name");
                table.AddHeaderCell("Date of Birth");
                table.AddHeaderCell("Email");
                table.AddHeaderCell("Address");

                // Populate table with patient data
                foreach (var user in await patients.ToListAsync())
                {
                    table.AddCell(user.FirstName);
                    table.AddCell(user.LastName);
                    table.AddCell(user.DateOfBirth.ToString("dd/MM/yyyy"));
                    table.AddCell(user.Email);
                    table.AddCell(user.Address);
                }

                // Add table to document
                document.Add(table);

                // Close document
                document.Close();

                // Return PDF file
                return File(memoryStream.ToArray(), "application/pdf", "PatientList.pdf");
            }
        }














        public async Task<IActionResult> AdmittedPatients(string searchString)
        {
            ViewData["ShowSidebar"] = true;

            // Fetch the admitted patients
            var admittedPatientsQuery = _context.AdmitPatients
                .Include(ap => ap.Patient)
                .Include(ap => ap.PatientAllergies).ThenInclude(pa => pa.Allergy)
                .Include(ap => ap.PatientMedications).ThenInclude(pm => pm.Medication)
                .Include(ap => ap.PatientConditions).ThenInclude(pc => pc.Condition)
                .AsQueryable();

            // Search logic
            if (!string.IsNullOrEmpty(searchString))
            {
                admittedPatientsQuery = admittedPatientsQuery.Where(ap =>
                    ap.Patient.FirstName.Contains(searchString) ||
                    ap.Patient.LastName.Contains(searchString) ||
                    ap.AdmitPatientStatus.ToString().Contains(searchString) ||
                    ap.AdmissionDate.ToString("yyyy-MM-dd").Contains(searchString));
            }

            var admittedPatients = await admittedPatientsQuery
                .Select(ap => new AdmittedPatientViewModel
                {
                    PatientId = ap.PatientId,
                    FullName = $"{ap.Patient.FirstName} {ap.Patient.LastName}",
                    AdmissionDate = ap.AdmissionDate,
                    Status = ap.AdmitPatientStatus,
                    Allergies = ap.PatientAllergies.Select(pa => pa.Allergy.Name).ToList(),
                    Medications = ap.PatientMedications.Select(pm => pm.Medication.Name).ToList(),
                    Conditions = ap.PatientConditions.Select(pc => pc.Condition.Name).ToList(),
                    NurseName = $"{ap.Nurse.FirstName} {ap.Nurse.LastName}"
                })
                .ToListAsync();

            // Create the statistics view model
            var statisticsViewModel = new AdmittedPatientsStatisticsViewModel
            {
                TotalPatients = admittedPatients.Count,
                ActivePatients = admittedPatients.Count(p => p.Status == TimelessTechnicians.UI.Models.AdmitPatientStatus.Admitted),
                DischargedPatients = admittedPatients.Count(p => p.Status == TimelessTechnicians.UI.Models.AdmitPatientStatus.Discharged),
                AdmittedPatients = admittedPatients
            };

            return View(statisticsViewModel);
        }


        public async Task<IActionResult> AdmitPatient()
        {
            ViewData["ShowSidebar"] = true;

            // Fetch only active patients
            var patients = await _context.Users
                .Where(u => u.Role == ApplicationUser.UserRole.PATIENT && u.Status == UserStatus.Active)
                .Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = $"{u.FirstName} {u.LastName}"
                })
                .ToListAsync();

            ViewBag.PatientList = patients;

            // Fetch available nurses who are not assigned to any patients
            var nurses = await _context.Users
                .Where(u => u.Role == ApplicationUser.UserRole.NURSE &&
                            u.Status == UserStatus.Active &&
                            !_context.AdmitPatients.Any(ap => ap.NurseId == u.Id && ap.AdmitPatientStatus == AdmitPatientStatus.Admitted))
                .Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = $"{u.FirstName} {u.LastName}"
                })
                .ToListAsync();

            ViewBag.NurseList = nurses;

            // Fetch allergies, medications, and conditions...
            var allergies = await _context.Allergies
                .Where(a => a.DeletionStatus == AllergyStatus.Active)
                .Select(a => new SelectListItem
                {
                    Value = a.AllergyId.ToString(),
                    Text = a.Name
                })
                .ToListAsync();

            ViewBag.AllergyList = allergies;

            var medications = await _context.Medications
                .Where(m => m.DeletionStatus == MedicationStatus.Active)
                .Select(m => new SelectListItem
                {
                    Value = m.MedicationId.ToString(),
                    Text = m.Name
                })
                .ToListAsync();

            ViewBag.MedicationList = medications;

            var conditions = await _context.Conditions
                .Where(c => c.DeletionStatus == ConditionStatus.Active)
                .Select(c => new SelectListItem
                {
                    Value = c.ConditionId.ToString(),
                    Text = c.Name
                })
                .ToListAsync();

            ViewBag.ConditionList = conditions;

            var model = new AdmitPatientViewModel
            {
                Status = AdmitPatientStatus.Admitted // Default status
            };

            return View(model);
        }



        [HttpPost]
        public async Task<IActionResult> AdmitPatient(AdmitPatientViewModel model)
        {
            ViewData["ShowSidebar"] = true;

            // Validation
            if (string.IsNullOrEmpty(model.PatientId) ||
                string.IsNullOrEmpty(model.SelectedNurseId) || // Ensure a nurse is selected
                !model.SelectedAllergies.Any() || // Check if any allergies are selected
                !model.SelectedMedications.Any() || // Check if any medications are selected
                !model.SelectedConditions.Any()) // Check if any conditions are selected
            {
                TempData["ErrorMessage"] = "Please ensure all required fields are filled out correctly.";

                // Re-populate the dropdowns before returning the view
                await PopulateDropdownsAsync(model);
                return View(model);
            }

            var existingAdmission = await _context.AdmitPatients
                 .FirstOrDefaultAsync(ap => ap.PatientId == model.PatientId && ap.AdmissionDate.Date == DateTime.Now.Date);

            if (existingAdmission != null)
            {
                TempData["PatientErrorMessage"] = "The patient is already admitted with the same admission date.";

                // Re-populate the dropdowns before returning the view
                await PopulateDropdownsAsync(model);
                return View(model);
            }

            // Check if the nurse is already assigned to another patient
            var isNurseAssigned = await _context.AdmitPatients
                .AnyAsync(ap => ap.NurseId == model.SelectedNurseId && ap.AdmitPatientStatus == AdmitPatientStatus.Admitted);

            if (isNurseAssigned)
            {
                TempData["ErrorMessage"] = "The selected nurse is already assigned to another patient.";

                // Re-populate the dropdowns before returning the view
                await PopulateDropdownsAsync(model);
                return View(model);
            }

            // Create an AdmitPatient instance with current date and time
            var admitPatient = new AdmitPatient
            {
                PatientId = model.PatientId,
                NurseId = model.SelectedNurseId,
                AdmissionDate = DateTime.Now, // Set to the current date and time
                AdmitPatientStatus = model.Status
            };

            // Add and save the AdmitPatient entity
            _context.AdmitPatients.Add(admitPatient);
            await _context.SaveChangesAsync();

            // Add selected allergies
            foreach (var allergyId in model.SelectedAllergies)
            {
                var patientAllergy = new PatientAllergy
                {
                    AdmitPatientId = admitPatient.Id,
                    AllergyId = allergyId,
                    PatientId = model.PatientId
                };
                _context.PatientAllergies.Add(patientAllergy);
            }

            // Add selected medications
            foreach (var medicationId in model.SelectedMedications)
            {
                var patientMedication = new PatientMedication
                {
                    AdmitPatientId = admitPatient.Id,
                    MedicationId = medicationId,
                    PatientId = model.PatientId,
                    DateAdministered = DateTime.Now // Set the administered date to now
                };
                _context.PatientMedications.Add(patientMedication);
            }

            // Add selected conditions
            foreach (var conditionId in model.SelectedConditions)
            {
                var patientCondition = new PatientCondition
                {
                    AdmitPatientId = admitPatient.Id,
                    ConditionId = conditionId,
                    PatientId = model.PatientId,
                    DateAdministered = DateTime.Now // Set the administered date to now
                };
                _context.PatientConditions.Add(patientCondition);
            }

            // Save the changes
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Patient admitted successfully.";
            return RedirectToAction(nameof(AdmittedPatients));
        }


        public async Task<IActionResult> OpenPatientFolder(string patientId)
        {
            ViewData["ShowSidebar"] = true;

            if (string.IsNullOrEmpty(patientId))
            {
                TempData["ErrorMessage"] = "Patient ID is required to open the folder.";
                return RedirectToAction(nameof(AdmittedPatients));
            }

            // Check if the patient exists
            var patient = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == patientId && u.Role == ApplicationUser.UserRole.PATIENT);

            if (patient == null)
            {
                TempData["ErrorMessage"] = "Patient not found.";
                return RedirectToAction(nameof(AdmittedPatients));
            }

            // Check if the patient folder exists
            var patientFolder = await _context.PatientFolders
                .FirstOrDefaultAsync(pf => pf.PatientId == patientId);

            if (patientFolder != null)
            {
                // Patient folder exists, redirect to the view showing the folder details
                TempData["WarningMessage"] = "Opening existing patient folder.";
                return RedirectToAction("ViewPatientFolder", new { id = patientFolder.Id });
            }

            // Create a new folder if it does not exist
            var nurseId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            patientFolder = new PatientFolder
            {
                PatientId = patientId,
                CreatedDate = DateTime.Now,
                NurseId = nurseId
            };

            _context.PatientFolders.Add(patientFolder);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "New patient folder created successfully.";

            // Fetch additional patient details
            var model = await GetPatientFolderDetails(patientId, patientFolder.Id);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditPatientFolder(string patientId)
        {
            ViewData["ShowSidebar"] = true; // Show the sidebar in the view

            if (string.IsNullOrEmpty(patientId))
            {
                TempData["ErrorMessage"] = "Patient ID is required.";
                return RedirectToAction(nameof(AdmittedPatients)); // Redirect if no patient ID
            }

            // Check if the patient exists
            var patient = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == patientId && u.Role == ApplicationUser.UserRole.PATIENT);

            if (patient == null)
            {
                TempData["ErrorMessage"] = "Patient not found.";
                return RedirectToAction(nameof(AdmittedPatients)); // Redirect back if patient not found
            }

            // Check if the patient folder exists
            var patientFolder = await _context.PatientFolders
                .FirstOrDefaultAsync(pf => pf.PatientId == patientId);

            if (patientFolder == null)
            {
                TempData["ErrorMessage"] = "Patient folder not found.";
                return RedirectToAction(nameof(AdmittedPatients)); // Redirect back if folder not found
            }

            // Fetch allergies, medications, and conditions for the patient
            var allergies = await _context.PatientAllergies
                .Where(pa => pa.PatientId == patientId)
                .Select(pa => pa.AllergyId)
                .ToListAsync();

            var medications = await _context.PatientMedications
                .Where(pm => pm.PatientId == patientId)
                .Select(pm => pm.MedicationId)
                .ToListAsync();

            var conditions = await _context.PatientConditions
                .Where(pc => pc.PatientId == patientId)
                .Select(pc => pc.ConditionId)
                .ToListAsync();

            // Prepare the model to pass to the view
            var model = new EditPatientFolderViewModel
            {
                PatientId = patient.Id,
                CreatedDate = patientFolder.CreatedDate,
                SelectedAllergies = allergies,
                SelectedMedications = medications,
                SelectedConditions = conditions
            };

            // Populate other necessary data if needed, like available nurses or allergies, medications, and conditions lists
            await PopulateDropdownsAsync(model);

            return View(model); // Return the edit view with the current patient folder data
        }

        [HttpPost]
        public async Task<IActionResult> EditPatientFolder(EditPatientFolderViewModel model)
        {
            ViewData["ShowSidebar"] = true; // Show the sidebar in the view

            // Validation
            if (model == null || string.IsNullOrEmpty(model.PatientId))
            {
                TempData["ErrorMessage"] = "Invalid patient data.";
                return RedirectToAction(nameof(AdmittedPatients)); // Redirect back if model is invalid
            }

            // Check if the patient exists
            var patient = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == model.PatientId && u.Role == ApplicationUser.UserRole.PATIENT);

            if (patient == null)
            {
                TempData["ErrorMessage"] = "Patient not found.";
                return RedirectToAction(nameof(AdmittedPatients)); // Redirect back if patient not found
            }

            // Check if the patient folder exists
            var patientFolder = await _context.PatientFolders
                .FirstOrDefaultAsync(pf => pf.PatientId == model.PatientId);

            if (patientFolder == null)
            {
                TempData["ErrorMessage"] = "Patient folder not found.";
                return RedirectToAction(nameof(AdmittedPatients)); // Redirect back if folder not found
            }

            // Update patient folder properties if needed
            patientFolder.CreatedDate = model.CreatedDate; // Update created date if needed

            // Save the changes to PatientFolder
            _context.PatientFolders.Update(patientFolder);
            await _context.SaveChangesAsync();

            // Update allergies
            var existingAllergies = await _context.PatientAllergies
                .Where(pa => pa.PatientId == model.PatientId)
                .ToListAsync();

            // Remove existing allergies not in the new selection
            foreach (var allergy in existingAllergies)
            {
                if (!model.SelectedAllergies.Contains(allergy.AllergyId))
                {
                    _context.PatientAllergies.Remove(allergy);
                }
            }

            // Add new allergies that are selected but not in existing
            foreach (var allergyId in model.SelectedAllergies)
            {
                if (!existingAllergies.Any(ea => ea.AllergyId == allergyId))
                {
                    var patientAllergy = new PatientAllergy
                    {
                        AdmitPatientId = patientFolder.Id, // Use the folder ID
                        AllergyId = allergyId,
                        PatientId = model.PatientId
                    };
                    _context.PatientAllergies.Add(patientAllergy);
                }
            }

            // Similar logic for medications
            var existingMedications = await _context.PatientMedications
                .Where(pm => pm.PatientId == model.PatientId)
                .ToListAsync();

            // Remove existing medications not in the new selection
            foreach (var medication in existingMedications)
            {
                if (!model.SelectedMedications.Contains(medication.MedicationId))
                {
                    _context.PatientMedications.Remove(medication);
                }
            }

            // Add new medications
            foreach (var medicationId in model.SelectedMedications)
            {
                if (!existingMedications.Any(em => em.MedicationId == medicationId))
                {
                    var patientMedication = new PatientMedication
                    {
                        AdmitPatientId = patientFolder.Id, // Use the folder ID
                        MedicationId = medicationId,
                        PatientId = model.PatientId,
                        DateAdministered = DateTime.Now // Or use a specific date if required
                    };
                    _context.PatientMedications.Add(patientMedication);
                }
            }

            // Similar logic for conditions
            var existingConditions = await _context.PatientConditions
                .Where(pc => pc.PatientId == model.PatientId)
                .ToListAsync();

            // Remove existing conditions not in the new selection
            foreach (var condition in existingConditions)
            {
                if (!model.SelectedConditions.Contains(condition.ConditionId))
                {
                    _context.PatientConditions.Remove(condition);
                }
            }

            // Add new conditions
            foreach (var conditionId in model.SelectedConditions)
            {
                if (!existingConditions.Any(ec => ec.ConditionId == conditionId))
                {
                    var patientCondition = new PatientCondition
                    {
                        AdmitPatientId = patientFolder.Id, // Use the folder ID
                        ConditionId = conditionId,
                        PatientId = model.PatientId,
                        DateAdministered = DateTime.Now // Or use a specific date if required
                    };
                    _context.PatientConditions.Add(patientCondition);
                }
            }

            // Save all changes
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Patient folder updated successfully.";
            return RedirectToAction(nameof(OpenPatientFolder), new { patientId = model.PatientId });
        }


        [HttpPost]
        public async Task<IActionResult> DeletePatientFolder(string patientId)
        {
            ViewData["ShowSidebar"] = true; // Show the sidebar in the view

            if (string.IsNullOrEmpty(patientId))
            {
                TempData["ErrorMessage"] = "Patient ID is required to delete the folder.";
                return RedirectToAction(nameof(AdmittedPatients)); // Redirect if no patient ID
            }

            // Check if the patient exists
            var patient = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == patientId && u.Role == ApplicationUser.UserRole.PATIENT);

            if (patient == null)
            {
                TempData["ErrorMessage"] = "Patient not found.";
                return RedirectToAction(nameof(AdmittedPatients)); // Redirect back if patient not found
            }

            // Check if the patient folder exists
            var patientFolder = await _context.PatientFolders
                .FirstOrDefaultAsync(pf => pf.PatientId == patientId);

            if (patientFolder == null)
            {
                TempData["ErrorMessage"] = "Patient folder not found.";
                return RedirectToAction(nameof(AdmittedPatients)); // Redirect back if folder not found
            }

            // Mark the patient folder as deleted
            patientFolder.Status = RecordStatus.Deleted;

            // Remove related allergies
            var allergies = await _context.PatientAllergies
                .Where(pa => pa.PatientId == patientId)
                .ToListAsync();
            foreach (var allergy in allergies)
            {
                allergy.Status = RecordStatus.Deleted; // Soft delete the allergy
            }

            // Remove related medications
            var medications = await _context.PatientMedications
                .Where(pm => pm.PatientId == patientId)
                .ToListAsync();
            foreach (var medication in medications)
            {
                medication.Status = RecordStatus.Deleted; // Soft delete the medication
            }

            // Remove related conditions
            var conditions = await _context.PatientConditions
                .Where(pc => pc.PatientId == patientId)
                .ToListAsync();
            foreach (var condition in conditions)
            {
                condition.Status = RecordStatus.Deleted; // Soft delete the condition
            }

            // Save all changes to the database
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Patient folder deleted successfully.";
            return RedirectToAction(nameof(AdmittedPatients)); // Redirect back to the admitted patients list
        }


        public async Task<IActionResult> ViewPatientFolder(string patientId)
        {
            ViewData["ShowSidebar"] = true;

            // Validate the patientId
            if (string.IsNullOrEmpty(patientId))
            {
                TempData["ErrorMessage"] = "Patient ID is required to view the folder.";
                return RedirectToAction(nameof(AdmittedPatients));
            }

            // Check if the patient exists
            var patient = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == patientId && u.Role == ApplicationUser.UserRole.PATIENT);

            if (patient == null)
            {
                TempData["ErrorMessage"] = "Patient not found.";
                return RedirectToAction(nameof(AdmittedPatients));
            }

            // Retrieve the patient folder using the PatientId
            var patientFolder = await _context.PatientFolders
                .FirstOrDefaultAsync(pf => pf.PatientId == patientId && pf.Status == RecordStatus.Active);

            if (patientFolder == null)
            {
                TempData["ErrorMessage"] = "Patient folder not found.";
                return RedirectToAction(nameof(AdmittedPatients));
            }

            // Check the user's role
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            bool isDoctor = userRole == ApplicationUser.UserRole.DOCTOR.ToString();

            // Fetch allergies, medications, and conditions
            var allergies = await _context.PatientAllergies
                .Where(pa => pa.PatientId == patientId && pa.Status != RecordStatus.Deleted)
                .Select(pa => pa.Allergy.Name)
                .ToListAsync();

            var medications = await _context.PatientMedications
                .Where(pm => pm.PatientId == patientId && pm.Status != RecordStatus.Deleted)
                .Select(pm => pm.Medication.Name)
                .ToListAsync();

            var conditions = await _context.PatientConditions
                .Where(pc => pc.PatientId == patientId && pc.Status != RecordStatus.Deleted)
                .Select(pc => pc.Condition.Name)
                .ToListAsync();

            // Fetch patient's recorded vitals
            var vitals = await _context.PatientVitals
                .Where(pv => pv.PatientId == patientId && pv.PatientVitalStatus != PatientVitalStatus.Deleted)
                .Select(pv => new PatientVitalListViewModel
                {
                    BloodPressure = pv.BloodPressure,
                    Temperature = pv.Temperature,
                    SugarLevel = pv.SugarLevel,
                    RecordedDate = pv.RecordedDate
                })
                .ToListAsync();

            // Fetch patient's recorded treatments
            var treatments = await _context.PatientTreatments
                .Where(t => t.PatientId == patientId && t.TreatmentStatus != TreatmentStatus.Deleted)
                .Select(t => new PatientTreatmentListViewModel
                {
                    Id = t.Id,
                    TreatmentDescription = t.TreatmentDescription,
                    DatePerformed = t.DatePerformed,
                    TreatmentStatus = t.TreatmentStatus
                })
                .ToListAsync();

            // Fetch patient's assigned bed and ward information
            var bedAssignment = await _context.BedAssignments
                .Include(ba => ba.Bed)
                .ThenInclude(b => b.Ward)
                .FirstOrDefaultAsync(ba => ba.AdmitPatient.PatientId == patientId && ba.BedAssignmentStatus == BedAssignmentStatus.Active);

            // Prepare the model to pass to the view
            var model = new ViewPatientFolderViewModel
            {
                PatientId = patient.Id.ToString(),
                PatientFolderId = patientFolder.Id,
                CreatedDate = patientFolder.CreatedDate,
                Allergies = allergies,
                Medications = medications,
                Conditions = conditions,
                Vitals = vitals,
                Treatments = treatments,
                FullName = $"{patient.FirstName} {patient.LastName}",
                BedNumber = bedAssignment?.Bed.BedNumber,
                WardName = bedAssignment?.Bed.Ward.WardName,
                IsDoctor = isDoctor,
                IsFolderDeleted = await _context.PatientFolders
                    .AnyAsync(pf => pf.PatientId == patientId && pf.Status == RecordStatus.Deleted) // Corrected line
            };

            // Return the view with the patient folder data
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> RestorePatientFolder(string patientId)
        {
            ViewData["ShowSidebar"] = true; // Show the sidebar in the view

            if (string.IsNullOrEmpty(patientId))
            {
                TempData["ErrorMessage"] = "Patient ID is required to restore the folder.";
                return RedirectToAction(nameof(AdmittedPatients)); // Redirect if no patient ID
            }

            // Check if the patient exists
            var patient = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == patientId && u.Role == ApplicationUser.UserRole.PATIENT);

            if (patient == null)
            {
                TempData["ErrorMessage"] = "Patient not found.";
                return RedirectToAction(nameof(AdmittedPatients)); // Redirect back if patient not found
            }

            // Check if the patient folder exists and is marked as deleted
            var patientFolder = await _context.PatientFolders
                .FirstOrDefaultAsync(pf => pf.PatientId == patientId && pf.Status == RecordStatus.Deleted);

            if (patientFolder == null)
            {
                TempData["ErrorMessage"] = "No deleted patient folder found.";
                return RedirectToAction(nameof(AdmittedPatients)); // Redirect back if no deleted folder found
            }

            // Restore the patient folder
            patientFolder.Status = RecordStatus.Active;

            // Restore related allergies
            var allergies = await _context.PatientAllergies
                .Where(pa => pa.PatientId == patientId && pa.Status == RecordStatus.Deleted)
                .ToListAsync();

            foreach (var allergy in allergies)
            {
                allergy.Status = RecordStatus.Active; // Restore the allergy
            }

            // Restore related medications
            var medications = await _context.PatientMedications
                .Where(pm => pm.PatientId == patientId && pm.Status == RecordStatus.Deleted)
                .ToListAsync();

            foreach (var medication in medications)
            {
                medication.Status = RecordStatus.Active; // Restore the medication
            }

            // Restore related conditions
            var conditions = await _context.PatientConditions
                .Where(pc => pc.PatientId == patientId && pc.Status == RecordStatus.Deleted)
                .ToListAsync();

            foreach (var condition in conditions)
            {
                condition.Status = RecordStatus.Active; // Restore the condition
            }

            // Save all changes to the database
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Patient folder restored successfully.";
            return RedirectToAction(nameof(AdmittedPatients)); // Redirect back to the admitted patients list
        }









        public async Task<IActionResult> ListPatientDischarges()
        {
            ViewData["ShowSidebar"] = true;

            var currentUserName = User.Identity.Name;

            var patientDischarges = await _context.PatientDischargeInstructions
                .Where(pd => pd.AdministeredBy == currentUserName && pd.Status != InstructionStatus.Deleted)
                .Include(pd => pd.AdmitPatient)
                    .ThenInclude(ap => ap.Patient)
                .ToListAsync();

            return View(patientDischarges);
        }




































        public async Task<IActionResult> ListAssignedBed()
        {
            ViewData["ShowSidebar"] = true;

            // Retrieve the list of active bed assignments including patient and bed details
            var bedAssignments = await _context.BedAssignments
                .Include(ba => ba.AdmitPatient)
                .ThenInclude(ap => ap.Patient)
                .Include(ba => ba.Bed)
                .ThenInclude(b => b.Ward)
                .Where(ba => ba.BedAssignmentStatus == BedAssignmentStatus.Active) // Filter active assignments
                .Select(ba => new BedAssignmentViewModel
                {
                    Id = ba.Id,
                    PatientName = $"{ba.AdmitPatient.Patient.FirstName} {ba.AdmitPatient.Patient.LastName}",
                    BedNumber = ba.Bed.BedNumber,
                    WardName = ba.Bed.Ward.WardName,
                    AssignedDate = ba.AssignedDate
                }).ToListAsync();

            // Prepare the view model
            var viewModel = new BedAssignmentListViewModel
            {
                BedAssignments = bedAssignments
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> AssignBed()
        {
            ViewData["ShowSidebar"] = true;

            // Retrieve the list of active patients
            var patients = await _context.AdmitPatients
                .Include(ap => ap.Patient)
                .Where(ap => ap.AdmitPatientStatus == AdmitPatientStatus.Admitted) // Filter for active patients
                .Select(ap => new SelectListItem
                {
                    Value = ap.Id.ToString(),
                    Text = $"{ap.Patient.FirstName} {ap.Patient.LastName} "
                }).ToListAsync();

            // Retrieve the list of available beds
            var availableBeds = await _context.Beds
                .Where(b => b.Status == BedStatus.Available)
                .Select(b => new SelectListItem
                {
                    Value = b.BedId.ToString(),
                    Text = $"Bed {b.BedNumber} - {b.Ward.WardName}"
                }).ToListAsync();

            // Prepare the view model
            var viewModel = new AssignBedViewModel
            {
                PatientList = patients,
                BedList = availableBeds
            };

            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> AssignBed(AssignBedViewModel model)
        {
            var admitPatient = await _context.AdmitPatients
                .Where(ap => ap.AdmitPatientStatus == AdmitPatientStatus.Admitted)
                .FirstOrDefaultAsync(ap => ap.Id == model.AdmitPatientId);

            if (admitPatient == null)
            {
                TempData["ErrorMessage"] = "The selected patient is not currently admitted.";
                return RedirectToAction(nameof(AssignBed));
            }

            var bed = await _context.Beds.FindAsync(model.BedId);
            if (bed == null || bed.Status != BedStatus.Available)
            {
                TempData["ErrorMessage"] = "The selected bed is unavailable. Please choose a different bed.";
                return RedirectToAction(nameof(AssignBed));
            }

            var bedAssignment = new BedAssignment
            {
                AdmitPatientId = model.AdmitPatientId,
                BedId = model.BedId,
                AssignedDate = DateTime.Now
            };

            bed.Status = BedStatus.Occupied;

            _context.BedAssignments.Add(bedAssignment);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Bed successfully assigned to the patient.";
            return RedirectToAction(nameof(ListAssignedBed));
        }


        [HttpGet]
        public async Task<IActionResult> EditAssignBed(int id)
        {
            ViewData["ShowSidebar"] = true;

            // Fetch the bed assignment record by its ID
            var bedAssignment = await _context.BedAssignments
                .Include(ba => ba.AdmitPatient)
                .ThenInclude(ap => ap.Patient)
                .Include(ba => ba.Bed)
                .ThenInclude(b => b.Ward)
                .FirstOrDefaultAsync(ba => ba.Id == id);

            if (bedAssignment == null)
            {
                return NotFound(); // Handle the case where the bed assignment record is not found
            }

            // Fetch active patients for the dropdown
            var patients = await _context.AdmitPatients
                .Where(ap => ap.AdmitPatientStatus == AdmitPatientStatus.Admitted)
                .Include(ap => ap.Patient)
                .Select(ap => new SelectListItem
                {
                    Value = ap.Id.ToString(),
                    Text = $"{ap.Patient.FirstName} {ap.Patient.LastName}"
                }).ToListAsync();

            // Fetch available beds for the dropdown
            var availableBeds = await _context.Beds
                .Where(b => b.Status == BedStatus.Available || b.BedId == bedAssignment.BedId) // Include the current bed in the list
                .Select(b => new SelectListItem
                {
                    Value = b.BedId.ToString(),
                    Text = $"Bed {b.BedNumber} - {b.Ward.WardName}"
                }).ToListAsync();

            // Prepare the view model with the existing bed assignment data
            var viewModel = new AssignBedViewModel
            {
                Id = bedAssignment.Id,
                AdmitPatientId = bedAssignment.AdmitPatientId,
                BedId = bedAssignment.BedId,
                AssignedDate = bedAssignment.AssignedDate,
                PatientList = patients,
                BedList = availableBeds
            };

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAssignBed(AssignBedViewModel model)
        {
            if (!BedAssignmentExists(model.Id))
            {
                TempData["ErrorMessage"] = "The bed assignment record could not be found.";
                return RedirectToAction(nameof(ListAssignedBed));
            }

            var bedAssignment = await _context.BedAssignments.FindAsync(model.Id);
            if (bedAssignment == null)
            {
                TempData["ErrorMessage"] = "The bed assignment record could not be found.";
                return RedirectToAction(nameof(ListAssignedBed));
            }

            var admitPatient = await _context.AdmitPatients
                .Where(ap => ap.AdmitPatientStatus == AdmitPatientStatus.Admitted)
                .FirstOrDefaultAsync(ap => ap.Id == model.AdmitPatientId);

            if (admitPatient == null)
            {
                TempData["ErrorMessage"] = "The selected patient is not currently admitted.";
                return RedirectToAction(nameof(ListAssignedBed));
            }

            var bed = await _context.Beds.FindAsync(model.BedId);
            if (bed == null || bed.Status != BedStatus.Available)
            {
                TempData["ErrorMessage"] = "The selected bed is unavailable. Please choose a different bed.";
                return RedirectToAction(nameof(ListAssignedBed));
            }

            bedAssignment.AdmitPatientId = model.AdmitPatientId;
            bedAssignment.BedId = model.BedId;
            bedAssignment.AssignedDate = model.AssignedDate;

            var oldBed = await _context.Beds.FindAsync(bedAssignment.BedId);
            if (oldBed != null)
            {
                oldBed.Status = BedStatus.Available;
            }

            bed.Status = BedStatus.Occupied;

            _context.Update(bedAssignment);
            _context.Update(bed);
            if (oldBed != null)
            {
                _context.Update(oldBed);
            }
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Bed assignment updated successfully.";
            return RedirectToAction(nameof(ListAssignedBed));
        }

        [HttpGet]
        public async Task<IActionResult> DeleteBedAssignment(int id)
        {
            ViewData["ShowSidebar"] = true;

            // Fetch the bed assignment record by its ID
            var bedAssignment = await _context.BedAssignments
                .Include(ba => ba.AdmitPatient)
                .ThenInclude(ap => ap.Patient)
                .Include(ba => ba.Bed)
                .ThenInclude(b => b.Ward)
                .FirstOrDefaultAsync(ba => ba.Id == id);

            if (bedAssignment == null)
            {
                return NotFound(); // Handle the case where the bed assignment record is not found
            }

            // Prepare the view model with the bed assignment details
            var viewModel = new BedAssignmentViewModel
            {
                Id = bedAssignment.Id,
                PatientName = $"{bedAssignment.AdmitPatient.Patient.FirstName} {bedAssignment.AdmitPatient.Patient.LastName}",
                BedNumber = bedAssignment.Bed.BedNumber,
                WardName = bedAssignment.Bed.Ward.WardName,
                AssignedDate = bedAssignment.AssignedDate
            };

            return View(viewModel);
        }

        [HttpPost, ActionName("ConfirmDeleteBedAssignment")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDeleteBedAssignment(int id)
        {
            var bedAssignment = await _context.BedAssignments
                .FirstOrDefaultAsync(ba => ba.Id == id);

            if (bedAssignment == null)
            {
                TempData["ErrorMessage"] = "The bed assignment record could not be found.";
                return RedirectToAction(nameof(ListAssignedBed));
            }

            bedAssignment.BedAssignmentStatus = BedAssignmentStatus.Deleted;

            var bed = await _context.Beds.FindAsync(bedAssignment.BedId);
            if (bed != null)
            {
                bed.Status = BedStatus.Available;
                _context.Update(bed);
            }

            _context.Update(bedAssignment);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Bed assignment deleted successfully.";
            return RedirectToAction(nameof(ListAssignedBed));
        }





        public IActionResult Discharge()
        {
            ViewData["ShowSidebar"] = true;

            // Populate view model with required data
            var model = new DischargeViewModel
            {
                AdmittedPatients = _context.AdmitPatients
                    .Where(p => p.AdmitPatientStatus == AdmitPatientStatus.Admitted)
                    .Select(p => new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = $"{p.Patient.FirstName} {p.Patient.LastName}"
                    }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Discharge(DischargeViewModel model)
        {

            var admitPatient = await _context.AdmitPatients.FindAsync(model.AdmitPatientId);
            if (admitPatient != null)
            {
                ViewData["ShowSidebar"] = true;
                admitPatient.DischargeDate = model.DischargeDate;
                admitPatient.DischargeNotes = model.Notes;
                admitPatient.AdmitPatientStatus = AdmitPatientStatus.Discharged;


                var bedAssignment = await _context.BedAssignments
                .FirstOrDefaultAsync(ba => ba.AdmitPatientId == admitPatient.Id);

                if (bedAssignment != null)
                {
                    var bed = await _context.Beds.FindAsync(bedAssignment.BedId);
                    if (bed != null)
                    {
                        bed.Status = BedStatus.Available;
                        _context.Update(bed); // Update the bed status in the context
                    }
                }


                var discharge = new Discharge
                {
                    AdmitPatientId = model.AdmitPatientId,
                    DischargeDate = model.DischargeDate,
                    Notes = model.Notes
                };

                _context.Add(discharge);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(DischargeList)); // Redirect to a suitable view
            }


            // If something goes wrong, reload view with data
            model.AdmittedPatients = _context.AdmitPatients
                .Where(p => p.AdmitPatientStatus == AdmitPatientStatus.Admitted)
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = $"{p.Patient.FirstName} {p.Patient.LastName}"
                }).ToList();

            return View(model);
        }
        public async Task<IActionResult> DischargeList(string searchTerm, DateTime? startDate, DateTime? endDate)
        {
            ViewData["ShowSidebar"] = true;

            // Base query with status filter
            var query = _context.Discharges
                .Include(d => d.AdmitPatient)
                .ThenInclude(ap => ap.Patient)
                .Where(d => d.DischargeStatus == DischargeStatus.Active) // Add this line
                .AsQueryable();

            // Apply search filter
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(d => d.AdmitPatient.Patient.FirstName.Contains(searchTerm) ||
                                          d.AdmitPatient.Patient.LastName.Contains(searchTerm));
            }

            // Apply date range filter
            if (startDate.HasValue)
            {
                query = query.Where(d => d.DischargeDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(d => d.DischargeDate <= endDate.Value);
            }

            // Execute query and prepare view model
            var discharges = await query
                .Select(d => new DischargeListViewModel
                {
                    Id = d.Id,
                    PatientName = $"{d.AdmitPatient.Patient.FirstName} {d.AdmitPatient.Patient.LastName}",
                    DischargeDate = d.DischargeDate,
                    Notes = d.Notes
                })
                .ToListAsync();

            var viewModel = new DischargeListViewModel
            {
                Discharges = discharges,
                SearchTerm = searchTerm,
                StartDate = startDate,
                EndDate = endDate
            };

            return View(viewModel);
        }



        public async Task<IActionResult> EditDischarge(int id)
        {
            ViewData["ShowSidebar"] = true;

            // Fetch the discharge record by its ID
            var discharge = await _context.Discharges
                .Include(d => d.AdmitPatient)
                .ThenInclude(ap => ap.Patient)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (discharge == null)
            {
                return NotFound(); // Handle the case where the discharge record is not found
            }

            // Fetch patients with active admission for the dropdown
            var admittedPatients = await _context.AdmitPatients
                .Where(ap => ap.AdmitPatientStatus == AdmitPatientStatus.Admitted)
                .Include(ap => ap.Patient)
                .Select(ap => new SelectListItem
                {
                    Value = ap.Id.ToString(),
                    Text = $"{ap.Patient.FirstName} {ap.Patient.LastName}"
                }).ToListAsync();

            // Prepare the view model with the existing discharge data
            var viewModel = new DischargeViewModel
            {
                Id = discharge.Id,
                AdmitPatientId = discharge.AdmitPatientId,
                AdmittedPatients = admittedPatients,
                DischargeDate = discharge.DischargeDate,
                Notes = discharge.Notes
            };

            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDischarge(DischargeViewModel model)
        {
            // Check if the discharge record exists
            if (!DischargeExists(model.Id))
            {
                return NotFound(); // Handle the case where the discharge record is not found
            }

            // Fetch the existing discharge record from the database
            var discharge = await _context.Discharges.FindAsync(model.Id);
            if (discharge == null)
            {
                return NotFound(); // Handle the case where the discharge record is not found
            }

            // Update the discharge details
            discharge.AdmitPatientId = model.AdmitPatientId;
            discharge.DischargeDate = model.DischargeDate;
            discharge.Notes = model.Notes;

            // Save the changes to the database
            _context.Update(discharge);
            TempData["SuccessMessage"] = "Patient discharged successfully.";
            await _context.SaveChangesAsync();

            // Redirect to the list of discharges or another appropriate page
            return RedirectToAction(nameof(DischargeList));

            // If model state is not valid, re-populate the dropdown list and return the form
            var admittedPatients = await _context.AdmitPatients
                .Where(ap => ap.AdmitPatientStatus == AdmitPatientStatus.Admitted)
                .Include(ap => ap.Patient)
                .Select(ap => new SelectListItem
                {
                    Value = ap.Id.ToString(),
                    Text = $"{ap.Patient.FirstName} {ap.Patient.LastName}"
                }).ToListAsync();

            model.AdmittedPatients = admittedPatients;
            TempData["ErrorMessage"] = "Error discharging the patient. Please try again.";
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> DeleteDischarge(int id)
        {
            ViewData["ShowSidebar"] = true;

            // Fetch the discharge record by its ID
            var discharge = await _context.Discharges
                .Include(d => d.AdmitPatient)
                .ThenInclude(ap => ap.Patient)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (discharge == null)
            {
                return NotFound(); // Handle the case where the discharge record is not found
            }

            // Prepare the view model with the discharge details
            var viewModel = new DischargeViewModel
            {
                Id = discharge.Id,
                PatientFullName = $"{discharge.AdmitPatient.Patient.FirstName} {discharge.AdmitPatient.Patient.LastName}",
                DischargeDate = discharge.DischargeDate,
                Notes = discharge.Notes
            };

            return View(viewModel);
        }

        [HttpPost, ActionName("ConfirmDeleteDischarge")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDeleteDischarge(int id)
        {
            var discharge = await _context.Discharges
                .FirstOrDefaultAsync(d => d.Id == id);

            if (discharge == null)
            {
                TempData["ErrorMessage"] = "Discharge record not found.";
                return RedirectToAction(nameof(DischargeList));
            }

            discharge.DischargeStatus = DischargeStatus.Deleted;

            _context.Update(discharge);
            await _context.SaveChangesAsync();

            TempData["SuccessMessageSuccessMessage"] = "Discharge record deleted successfully.";
            return RedirectToAction(nameof(DischargeList));
        }
























        // GET: ReAdmit
        public IActionResult ReAdmit()
        {
            ViewData["ShowSidebar"] = true;

            var model = new ReAdmitPatientViewModel
            {
                DischargedPatients = _context.AdmitPatients
                    .Where(p => p.AdmitPatientStatus == AdmitPatientStatus.Discharged)
                    .Select(p => new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = $"{p.Patient.FirstName} {p.Patient.LastName} (Discharged on: {p.DischargeDate})"
                    }).ToList()
            };

            return View(model);
        }


        // POST: ReAdmit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReAdmit(ReAdmitPatientViewModel model)
        {
            var admitPatient = await _context.AdmitPatients.FindAsync(model.AdmitPatientId);
            if (admitPatient != null && admitPatient.AdmitPatientStatus == AdmitPatientStatus.Discharged)
            {
                admitPatient.AdmissionDate = DateTime.Now;
                admitPatient.AdmitPatientStatus = AdmitPatientStatus.Admitted;
                admitPatient.DischargeDate = null;
                admitPatient.DischargeNotes = null;

                // Handle bed assignment
                var bedAssignment = await _context.BedAssignments
                    .FirstOrDefaultAsync(ba => ba.AdmitPatientId == admitPatient.Id);

                if (bedAssignment != null)
                {
                    var bed = await _context.Beds.FindAsync(bedAssignment.BedId);
                    if (bed != null)
                    {
                        bed.Status = BedStatus.Occupied;
                        _context.Update(bed);
                    }
                }

                // Add re-admission history record
                var reAdmissionHistory = new ReAdmissionHistory
                {
                    AdmitPatientId = admitPatient.Id,
                    ReAdmissionDate = DateTime.Now,
                    Reason = model.ReAdmissionReason
                };

                _context.Add(reAdmissionHistory);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Patient successfully re-admitted.";
                return RedirectToAction(nameof(ReAdmissionHistory));
            }

            // Reload the form with discharged patients
            model.DischargedPatients = _context.AdmitPatients
                .Where(p => p.AdmitPatientStatus == AdmitPatientStatus.Discharged)
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = $"{p.Patient.FirstName} {p.Patient.LastName} (Discharged on: {p.DischargeDate})"
                }).ToList();

            TempData["ErrorMessage"] = "Error re-admitting the patient. Please check the details.";
            return View(model);
        }


        // GET: ReAdmissionHistory
        public IActionResult ReAdmissionHistory(string searchTerm, int page = 1)
        {
            ViewData["ShowSidebar"] = true;

            int pageSize = 10; // Number of records per page
            var query = _context.ReAdmissionHistories
                .Include(r => r.AdmitPatient)
                    .ThenInclude(p => p.Patient)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(r => r.AdmitPatient.Patient.FirstName.Contains(searchTerm)
                                      || r.AdmitPatient.Patient.LastName.Contains(searchTerm)
                                      || r.Reason.Contains(searchTerm));
            }

            var totalRecords = query.Count();
            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            var history = query
                .OrderByDescending(r => r.ReAdmissionDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var model = new ReAdmissionHistoryViewModel
            {
                SearchTerm = searchTerm,
                ReAdmissionHistories = history,
                CurrentPage = page,
                TotalPages = totalPages
            };

            return View(model);
        }




































        public async Task<IActionResult> ManagePatientMovements()
        {
            ViewData["ShowSidebar"] = true;

            // Fetch all active patient movements from the database
            var movements = await _context.PatientMovements
                .Where(pm => pm.MovementStatus == MovementStatus.Active) // Corrected reference
                .Include(pm => pm.AdmitPatient)
                .ThenInclude(ap => ap.Patient)
                .OrderByDescending(pm => pm.MovementDate)
                .ToListAsync();

            var viewModel = movements.Select(pm => new PatientMovementViewModel
            {
                Id = pm.Id,
                PatientFullName = $"{pm.AdmitPatient.Patient.FirstName} {pm.AdmitPatient.Patient.LastName}",
                MovementDate = pm.MovementDate,
                Location = pm.Location,
                Notes = pm.Notes
            }).ToList();

            return View(viewModel);
        }


        [HttpGet]
        public async Task<IActionResult> RecordMovement()
        {
            ViewData["ShowSidebar"] = true;

            // Fetch patients with active admission
            var patients = await _context.AdmitPatients
                .Include(ap => ap.Patient)
                .Select(ap => new SelectListItem
                {
                    Value = ap.Id.ToString(),
                    Text = $"{ap.Patient.FirstName} {ap.Patient.LastName}"
                }).ToListAsync();

            // Prepare the view model
            var viewModel = new PatientMovementViewModel
            {
                PatientList = patients,
                MovementDate = DateTime.Now,
                MovementType = ViewModel.MovementType.CheckIn, // Default value
                Location="",
                Notes= ""
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> RecordMovement(PatientMovementViewModel model)
        {
            
                // Create a new PatientMovement instance
                var movement = new PatientMovement
                {
                    AdmitPatientId = model.AdmitPatientId,
                    MovementDate = model.MovementDate,
                    Location = model.Location,
                    Notes = model.Notes
                };

                // Add the movement record to the database
                _context.PatientMovements.Add(movement);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Patient movement recorded successfully.";
                return RedirectToAction(nameof(ManagePatientMovements));
            

            // If model state is not valid, fetch patients again and return to the form
            var patients = await _context.AdmitPatients
                .Include(ap => ap.Patient)
                .Select(ap => new SelectListItem
                {
                    Value = ap.Id.ToString(),
                    Text = $"{ap.Patient.FirstName} {ap.Patient.LastName}"
                }).ToListAsync();

            model.PatientList = patients;
            TempData["ErrorMessage"] = "Error recording patient movement. Please check the details.";
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditMovement(int id)
        {
            ViewData["ShowSidebar"] = true;

            // Fetch the movement record by its ID
            var movement = await _context.PatientMovements
                .Include(pm => pm.AdmitPatient)
                .ThenInclude(ap => ap.Patient)
                .FirstOrDefaultAsync(pm => pm.Id == id);

            if (movement == null)
            {
                return NotFound(); // Handle the case where the movement record is not found
                ViewData["ShowSidebar"] = true;

            }

            // Fetch patients with active admission for the dropdown
            var patients = await _context.AdmitPatients
                .Include(ap => ap.Patient)
                .Select(ap => new SelectListItem
                {
                    Value = ap.Id.ToString(),
                    Text = $"{ap.Patient.FirstName} {ap.Patient.LastName}"
                }).ToListAsync();

            // Prepare the view model with the existing movement data
            var viewModel = new PatientMovementViewModel
            {
                Id = movement.Id,
                AdmitPatientId = movement.AdmitPatientId,
                PatientList = patients,
                MovementDate = movement.MovementDate,
                Location = movement.Location,
                Notes = movement.Notes
            };

            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMovement(PatientMovementViewModel model)
        {
            if (!PatientMovementExists(model.Id))
            {
                return NotFound();
            }

            var movement = await _context.PatientMovements.FindAsync(model.Id);
            if (movement == null)
            {
                return NotFound();
            }

            // Update the movement details
            movement.AdmitPatientId = model.AdmitPatientId;
            movement.MovementDate = model.MovementDate;
            movement.Location = model.Location;
            movement.Notes = model.Notes;

            // Save the changes to the database
            _context.Update(movement);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Patient movement updated successfully.";
            return RedirectToAction(nameof(ManagePatientMovements));

            // If model state is not valid, re-populate the dropdown list and return the form
            var patients = await _context.AdmitPatients
                .Include(ap => ap.Patient)
                .Select(ap => new SelectListItem
                {
                    Value = ap.Id.ToString(),
                    Text = $"{ap.Patient.FirstName} {ap.Patient.LastName}"
                }).ToListAsync();

            model.PatientList = patients;
            TempData["ErrorMessage"] = "Error updating patient movement. Please check the details.";
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> DeleteMovement(int id)
        {
            ViewData["ShowSidebar"] = true;
            var movement = await _context.PatientMovements
                .Include(pm => pm.AdmitPatient)
                .ThenInclude(ap => ap.Patient)
                .FirstOrDefaultAsync(pm => pm.Id == id);

            if (movement == null)
            {
                return NotFound(); // Handle the case where the movement record is not found
            }

            var viewModel = new PatientMovementViewModel
            {
                Id = movement.Id,
                PatientFullName = $"{movement.AdmitPatient.Patient.FirstName} {movement.AdmitPatient.Patient.LastName}",
                MovementDate = movement.MovementDate,
                Location = movement.Location,
                Notes = movement.Notes
            };

            return View(viewModel);
        }

        [HttpPost, ActionName("DeleteMovement")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDeleteMovement(int id)
        {
            var movement = await _context.PatientMovements
                .FirstOrDefaultAsync(pm => pm.Id == id);

            if (movement == null)
            {
                return NotFound();
            }

            movement.MovementStatus = MovementStatus.Deleted; // Soft delete

            _context.Update(movement);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Patient movement deleted successfully.";
            return RedirectToAction(nameof(ManagePatientMovements));
        }














        // Method to refetch dropdown data
        private async Task PopulateDropdownsAsync(AdmitPatientViewModel model)
        {
            // Fetch and order patients alphabetically by first name and last name
            var patients = await _context.Users
                .Where(u => u.Role == ApplicationUser.UserRole.PATIENT)
                .OrderBy(u => u.FirstName).ThenBy(u => u.LastName)
                .Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = $"{u.FirstName} {u.LastName}"
                })
                .ToListAsync();

            ViewBag.PatientList = patients;

            // Fetch and order allergies alphabetically by name
            var allergies = await _context.Allergies
                .Where(a => a.DeletionStatus == AllergyStatus.Active)
                .OrderBy(a => a.Name)
                .Select(a => new SelectListItem
                {
                    Value = a.AllergyId.ToString(),
                    Text = a.Name
                })
                .ToListAsync();

            ViewBag.AllergyList = allergies;

            // Fetch and order medications alphabetically by name
            var medications = await _context.Medications
                .Where(m => m.DeletionStatus == MedicationStatus.Active)
                .OrderBy(m => m.Name)
                .Select(m => new SelectListItem
                {
                    Value = m.MedicationId.ToString(),
                    Text = m.Name
                })
                .ToListAsync();

            ViewBag.MedicationList = medications;

            // Fetch and order conditions alphabetically by name
            var conditions = await _context.Conditions
                .Where(c => c.DeletionStatus == ConditionStatus.Active)
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem
                {
                    Value = c.ConditionId.ToString(),
                    Text = c.Name
                })
                .ToListAsync();

            ViewBag.ConditionList = conditions;

            model.NurseList = await _context.Users
     .Where(u => u.Role == ApplicationUser.UserRole.NURSE && u.Status == UserStatus.Active) // Adjust this based on your user model
     .Select(n => new SelectListItem
     {
         Value = n.Id,
         Text = $"{n.FirstName} {n.LastName}"
     })
     .ToListAsync();


          
        }


        private async Task PopulateDropdownsAsync(EditPatientFolderViewModel model)
        {
          
           
            model.Allergies = await _context.Allergies.ToListAsync(); // Assuming you have an Allergies property in your ViewModel
            model.Medications = await _context.Medications.ToListAsync(); // Assuming you have a Medications property in your ViewModel
            model.Conditions = await _context.Conditions.ToListAsync(); // Assuming you have a Conditions property in your ViewModel
        }


        private bool PatientMovementExists(int id)
        {
            return _context.PatientMovements.Any(e => e.Id == id);
        }

        private bool DischargeExists(int id)
        {
            return _context.Discharges.Any(e => e.Id == id);
        }
       
        private bool BedAssignmentExists(int id)
        {
            return _context.BedAssignments.Any(e => e.Id == id);
        }

        private bool AdmitPatientExists(int id)
        {
            return _context.AdmitPatients.Any(e => e.Id == id);
        }

        private async Task PopulateDropdownsAsync()
        {
            var allergies = await _context.Allergies
                .Where(a => a.DeletionStatus == AllergyStatus.Active)
                .Select(a => new SelectListItem { Value = a.AllergyId.ToString(), Text = a.Name })
                .ToListAsync();
            ViewBag.AllergyList = allergies;

            var medications = await _context.Medications
                .Where(m => m.DeletionStatus == MedicationStatus.Active)
                .Select(m => new SelectListItem { Value = m.MedicationId.ToString(), Text = m.Name })
                .ToListAsync();
            ViewBag.MedicationList = medications;

            var conditions = await _context.Conditions
                .Where(c => c.DeletionStatus == ConditionStatus.Active)
                .Select(c => new SelectListItem { Value = c.ConditionId.ToString(), Text = c.Name })
                .ToListAsync();
            ViewBag.ConditionList = conditions;
        }


        private async Task<PatientFolderViewModel> GetPatientFolderDetails(string patientId, int patientFolderId)
        {
            // Fetch allergies, medications, and conditions for the patient
            var allergies = await _context.PatientAllergies
                .Where(pa => pa.PatientId == patientId)
                .Include(pa => pa.Allergy)
                .ToListAsync();

            var medications = await _context.PatientMedications
                .Where(pm => pm.PatientId == patientId)
                .Include(pm => pm.Medication)
                .ToListAsync();

            var conditions = await _context.PatientConditions
                .Where(pc => pc.PatientId == patientId)
                .Include(pc => pc.Condition)
                .ToListAsync();

            var model = new PatientFolderViewModel
            {
                PatientId = patientId,
                // Fill in additional properties as needed
                Allergies = allergies.Select(a => a.Allergy.Name).ToList(),
                Medications = medications.Select(m => new MedicationViewModel
                {
                    Name = m.Medication.Name,
                }).ToList(),
                Conditions = conditions.Select(c => new ConditionViewModel
                {
                    Name = c.Condition.Name,
                }).ToList(),
            };

            return model;
        }
    }
}



