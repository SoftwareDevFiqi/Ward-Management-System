using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TimelessTechnicians.UI.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using TimelessTechnicians.UI.ViewModel;
using static TimelessTechnicians.UI.Models.ApplicationUser;
using TimelessTechnicians.UI.Data;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using OfficeOpenXml;
using iText.Layout.Properties;
using iText.IO.Image;
using iText.Kernel.Geom;
using Path = System.IO.Path;



namespace TimelessTechnicians.UI.Controllers
{
    [Authorize(Roles = "ADMINISTRATOR,CONSUMABLESMANAGER")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const double CapacityThreshold = 0.10; 

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> AdminDashboard()
        {
            ViewData["ShowSidebar"] = true;
            var model = new AdminDashboardViewModel
            {
                // Patient Statistics
                TotalPatients = await _context.Users
                    .CountAsync(u => u.Role == UserRole.PATIENT && u.Status == UserStatus.Active),

                PatientsAdmittedToday = await _context.AdmitPatients
                    .Where(ap => ap.AdmissionDate.Date == DateTime.Today)
                    .CountAsync(ap => _context.Users
                        .Any(u => u.Id == ap.PatientId && u.Role == UserRole.PATIENT && u.Status == UserStatus.Active)),

                // Bed Availability
                TotalBeds = await _context.Beds.CountAsync(b => b.DeletionStatus == CondtionStatus.Active),
                AvailableBeds = await _context.Beds.CountAsync(b => b.Status == BedStatus.Available),
                OccupiedBeds = await _context.Beds.CountAsync(b => b.Status == BedStatus.Occupied),

                // Allergy Statistics
                TotalAllergies = await _context.Allergies.CountAsync(a => a.DeletionStatus == AllergyStatus.Active),
                ActiveAllergies = await _context.Allergies.CountAsync(a => a.DeletionStatus == AllergyStatus.Active),

                // Medication Overview
                TotalMedications = await _context.Medications.CountAsync(m => m.DeletionStatus == MedicationStatus.Active),

                // Additional Statistics
                TotalWards = await _context.Wards.CountAsync(w => w.WardStatus == WardStatus.Active),
                TotalConditions = await _context.Conditions.CountAsync(c => c.DeletionStatus == ConditionStatus.Active),
                TotalConsumables = await _context.Consumables.CountAsync(c => c.DeletionStatus == ConsumableStatus.Active),

                // Count Active Users
                ActiveUsers = await _context.Users.CountAsync(u => u.Status == UserStatus.Active),

                // Employee Statistics
                TotalDoctors = await _context.Users.CountAsync(u => u.Role == UserRole.DOCTOR && u.Status == UserStatus.Active),
                TotalWardAdmins = await _context.Users.CountAsync(u => u.Role == UserRole.WARDADMIN && u.Status == UserStatus.Active),
                TotalNurses = await _context.Users.CountAsync(u => u.Role == UserRole.NURSE && u.Status == UserStatus.Active),
                TotalNursingSisters = await _context.Users.CountAsync(u => u.Role == UserRole.NURSINGSISTER && u.Status == UserStatus.Active),
                TotalScriptManagers = await _context.Users.CountAsync(u => u.Role == UserRole.SCRIPTMANAGER && u.Status == UserStatus.Active),
                TotalConsumableManagers = await _context.Users.CountAsync(u => u.Role == UserRole.CONSUMABLESMANAGER && u.Status == UserStatus.Active),

               
            };

            return View(model);
        }






























        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkDeleteWards(List<int> selectedWards)
        {
            if (selectedWards == null || !selectedWards.Any())
            {
                TempData["ErrorMessage"] = "No wards were selected for deletion.";
                return RedirectToAction(nameof(Wards));
            }

            try
            {
                foreach (var wardId in selectedWards)
                {
                    var ward = await _context.Wards
                        .Include(w => w.Beds) // Include related beds for deletion
                        .FirstOrDefaultAsync(w => w.WardId == wardId && w.WardStatus == WardStatus.Active);
                    if (ward != null)
                    {
                        // Soft delete beds
                        foreach (var bed in ward.Beds)
                        {
                            bed.DeletionStatus = CondtionStatus.Deleted;
                            _context.Update(bed);
                        }

                        // Soft delete ward
                        ward.WardStatus = WardStatus.Deleted;
                        _context.Update(ward);
                    }
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Selected wards deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the selected wards.";
                // Optionally log the error
            }

            return RedirectToAction(nameof(Wards));
        }



        // Wards Actions

        public async Task<IActionResult> Wards(int pageNumber = 1, int pageSize = 4, string sortOrder = "", string searchString = "", string descriptionString = "", int? id = null)
        {
            ViewData["ShowSidebar"] = true;

            // Create a base query for wards with active status
            IQueryable<Ward> wardsQuery = _context.Wards
                .Where(w => w.WardStatus == WardStatus.Active)
                .Include(w => w.Beds);

            // Search functionality for multiple WardNames and Description
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                // Split the search string by commas and trim whitespace
                var searchTerms = searchString.Split(',')
                    .Select(term => term.Trim())
                    .Where(term => !string.IsNullOrEmpty(term))
                    .ToList();

                // Filter wards that match any of the search terms
                if (searchTerms.Any())
                {
                    wardsQuery = wardsQuery.Where(w => searchTerms.Any(term => w.WardName.Contains(term)));
                }
            }

            if (!string.IsNullOrWhiteSpace(descriptionString))
            {
                wardsQuery = wardsQuery.Where(w => w.Description.Contains(descriptionString));
            }

            // Determine the sorting order
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : ""; // Toggle sort order
            ViewData["CapacitySortParm"] = sortOrder == "capacity" ? "capacity_desc" : "capacity"; // Toggle capacity sort order
            ViewData["NewWardId"] = id; // Now 'id' is defined and can be used here

            switch (sortOrder)
            {
                case "name_desc":
                    wardsQuery = wardsQuery.OrderByDescending(w => w.WardName);
                    break;
                case "capacity":
                    wardsQuery = wardsQuery.OrderBy(w => w.Capacity);
                    break;
                case "capacity_desc":
                    wardsQuery = wardsQuery.OrderByDescending(w => w.Capacity);
                    break;
                default: // WardName ascending 
                    wardsQuery = wardsQuery.OrderBy(w => w.WardName);
                    break;
            }

            // Pagination
            var totalWards = await wardsQuery.CountAsync();
            var totalPages = (int)Math.Ceiling(totalWards / (double)pageSize);

            var wards = await wardsQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Calculate remaining capacity
            foreach (var ward in wards)
            {
                ward.RemainingCapacity = ward.Capacity - ward.Beds.Count;

                // Check if remaining capacity is below the threshold
                if (ward.RemainingCapacity < (ward.Capacity * CapacityThreshold))
                {
                    TempData["WarningMessage"] = $"Warning: Remaining capacity for ward '{ward.WardName}' is below 10%.";
                }
            }

            ViewData["PageNumber"] = pageNumber;
            ViewData["TotalPages"] = totalPages;
            ViewData["CurrentSort"] = sortOrder;
            ViewData["CurrentSearch"] = searchString; // Store current search string for the view
            ViewData["CurrentDescription"] = descriptionString; // Store current description string for the view

            return View(wards);
        }

        [HttpGet]
        public async Task<IActionResult> GetWardNames(string term)
        {
            var wardNames = await _context.Wards
                .Where(w => w.WardStatus == WardStatus.Active && w.WardName.Contains(term))
                .Select(w => w.WardName)
                .ToListAsync();

            return Json(wardNames);
        }


        public IActionResult AddWard()
        {
            ViewData["ShowSidebar"] = true;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddWard([Bind("WardId,WardName,Description,Capacity")] Ward ward)
        {
            try
            {
                // Check if a ward with the same name already exists
                var existingWard = await _context.Wards
                    .Where(w => w.WardStatus == WardStatus.Active && w.WardName.ToLower() == ward.WardName.ToLower())
                    .FirstOrDefaultAsync();

                if (existingWard != null)
                {
                    // If a ward with the same name exists, return an error message
                    TempData["ErrorMessage"] = $"A ward with the name '{ward.WardName}' already exists. Please choose a different name.";
                    return View(ward); // Return the view with the current model state
                }

                // Set default status and save the new ward
                ward.WardStatus = WardStatus.Active;
                _context.Add(ward);
                await _context.SaveChangesAsync();

                // Set a success message in TempData
                TempData["SuccessMessage"] = $"Ward '{ward.WardName}' was successfully created. You can now assign beds to it.";

                // Redirect with WardId to show the tick icon for the newly added ward
                return RedirectToAction(nameof(Wards), new { id = ward.WardId });
            }
            catch (Exception ex)
            {
                // Return a general error message in case of exception
                TempData["ErrorMessage"] = $"An error occurred while adding the ward '{ward.WardName}'. Please verify the input and try again.";
                return View(ward);
            }
        }



        public async Task<IActionResult> EditWard(int? id)
        {
            if (id == null)
            {
                TempData["WardErrorMessage"] = "Ward not found.";
                return NotFound();
            }
            ViewData["ShowSidebar"] = true;

            var ward = await _context.Wards.FindAsync(id);
            if (ward == null || ward.WardStatus == WardStatus.Deleted)
            {
                TempData["WardErrorMessage"] = "Ward not found or has been deleted.";
                return NotFound();
            }
            return View(ward);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditWard(int id, [Bind("WardId,WardName,Description,DeletionStatus,Capacity")] Ward ward)
        {
            if (id != ward.WardId)
            {
                TempData["ErrorMessage"] = "Invalid ward ID.";
                return NotFound();
            }

            // Check for duplication of WardName (excluding the current ward being edited)
            if (_context.Wards.Any(w => w.WardName == ward.WardName && w.WardId != id && w.WardStatus == WardStatus.Active))
            {
                TempData["ErrorMessage"] = "A ward with the same name already exists.";
                return View(ward); // Return the view with the current ward data to correct the error
            }

            try
            {
                _context.Update(ward);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Ward updated successfully!";
                return RedirectToAction(nameof(Wards));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WardExists(ward.WardId))
                {
                    TempData["WardErrorMessage"] = "Ward not found.";
                    return NotFound();
                }
                else
                {
                    TempData["WardErrorMessage"] = "An error occurred while updating the ward.";
                    throw;
                }
            }
        }

        public async Task<IActionResult> DeleteWard(int? id)
        {
            if (id == null)
            {
                TempData["WardErrorMessage"] = "Ward not found.";
                return NotFound();
            }
            ViewData["ShowSidebar"] = true;

            var ward = await _context.Wards
                .FirstOrDefaultAsync(m => m.WardId == id && m.WardStatus == WardStatus.Active);
            if (ward == null)
            {
                TempData["WardErrorMessage"] = "Ward not found or has already been deleted.";
                return NotFound();
            }

            return View(ward);
        }

        [HttpPost, ActionName("DeleteWard")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteWardConfirmed(int id)
        {
            var ward = await _context.Wards
                .Include(w => w.Beds)
                .FirstOrDefaultAsync(w => w.WardId == id && w.WardStatus == WardStatus.Active);

            if (ward == null)
            {
                TempData["ErrorMessage"] = "Ward not found or has already been deleted.";
                return NotFound();
            }

            // Soft delete beds
            foreach (var bed in ward.Beds)
            {
                bed.DeletionStatus = CondtionStatus.Deleted;
                _context.Update(bed);
            }

            // Soft delete ward
            ward.WardStatus = WardStatus.Deleted;
            _context.Update(ward);

            // Save changes
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Ward deleted successfully!";
            return RedirectToAction(nameof(Wards));
        }

        private bool WardExists(int id)
        {
            return _context.Wards.Any(e => e.WardId == id && e.WardStatus == WardStatus.Active);
        }

        [HttpGet]
        public async Task<IActionResult> ExportWardsToPdf(string orientation = "portrait", string paperSize = "A4", int fontSize = 8)
        {
            try
            {
                // Get hospital information
                var hospitalInfo = await _context.HospitalInfos.FirstOrDefaultAsync();

                if (hospitalInfo == null)
                {
                    return BadRequest("Hospital information is missing.");
                }

                // Get wards data
                var wards = await _context.Wards
                    .Where(w => w.WardStatus == WardStatus.Active)
                    .Include(w => w.Beds)
                    .ToListAsync();

                using (var memoryStream = new MemoryStream())
                {
                    var pdfWriter = new PdfWriter(memoryStream);

                    // Set the page size and orientation
                    PageSize pageSize;
                    switch (paperSize.ToLower())
                    {
                        case "letter":
                            pageSize = PageSize.LETTER;
                            break;
                        default:
                            pageSize = PageSize.A4;
                            break;
                    }

                    // Adjust for landscape orientation if selected
                    if (orientation.ToLower() == "landscape")
                    {
                        pageSize = pageSize.Rotate();
                    }

                    using (var pdfDocument = new PdfDocument(pdfWriter))
                    {
                        var document = new Document(pdfDocument, pageSize);
                        document.SetMargins(20, 20, 20, 20); // Set margins

                        // Add the hospital logo
                        var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "LogoResized.png");
                        if (System.IO.File.Exists(logoPath))
                        {
                            var logoImageData = ImageDataFactory.Create(logoPath);
                            var logo = new iText.Layout.Element.Image(logoImageData)
                                .SetWidth(50)
                                .SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.LEFT)
                                .SetMarginBottom(10);
                            document.Add(logo);
                        }

                        // Add hospital info
                        var hospitalInfoParagraph = new Paragraph($"{hospitalInfo.HospitalName}\n{hospitalInfo.Address}\n{hospitalInfo.PhoneNumber}")
                            .SetFontSize(fontSize) // Use dynamic font size
                            .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
                            .SetMarginBottom(10);
                        document.Add(hospitalInfoParagraph);

                        // Add header
                        document.Add(new Paragraph("Wards Data Export")
                            .SetBold()
                            .SetFontSize(fontSize + 8) // Slightly larger font for header
                            .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                            .SetMarginBottom(10));

                        // Create table with 5 columns
                        var table = new Table(UnitValue.CreatePercentArray(new float[] { 1, 2, 3, 1, 1 }))
                            .UseAllAvailableWidth(); // Full width table

                        // Set table headers with dynamic font size
                        table.AddHeaderCell(new Cell().Add(new Paragraph("Ward ID").SetFontSize(fontSize)));
                        table.AddHeaderCell(new Cell().Add(new Paragraph("Name").SetFontSize(fontSize)));
                        table.AddHeaderCell(new Cell().Add(new Paragraph("Description").SetFontSize(fontSize)));
                        table.AddHeaderCell(new Cell().Add(new Paragraph("Capacity").SetFontSize(fontSize)));
                        table.AddHeaderCell(new Cell().Add(new Paragraph("Remaining Capacity").SetFontSize(fontSize)));

                        // Populate table with ward data
                        int totalBeds = 0;
                        foreach (var ward in wards)
                        {
                            ward.RemainingCapacity = ward.Capacity - ward.Beds.Count; // Calculate remaining capacity

                            table.AddCell(new Cell().Add(new Paragraph(ward.WardId.ToString()).SetFontSize(fontSize)));
                            table.AddCell(new Cell().Add(new Paragraph(ward.WardName).SetFontSize(fontSize)));
                            table.AddCell(new Cell().Add(new Paragraph(ward.Description).SetFontSize(fontSize)));
                            table.AddCell(new Cell().Add(new Paragraph(ward.Capacity.ToString()).SetFontSize(fontSize)));
                            table.AddCell(new Cell().Add(new Paragraph(ward.RemainingCapacity.ToString()).SetFontSize(fontSize)));

                            totalBeds += ward.Beds.Count;
                        }

                        // Add table to document
                        document.Add(table);

                        // Summary section
                        document.Add(new Paragraph("\nSummary:")
                            .SetBold()
                            .SetFontSize(fontSize + 2) // Slightly larger for summary
                            .SetMarginTop(10));
                        document.Add(new Paragraph($"Total Wards: {wards.Count}").SetFontSize(fontSize));
                        document.Add(new Paragraph($"Total Beds: {totalBeds}").SetFontSize(fontSize));
                        document.Add(new Paragraph($"Generated on: {DateTime.Now.ToString("MM/dd/yyyy HH:mm")}")
                            .SetFontSize(fontSize));

                        document.Close();
                    }

                    return File(memoryStream.ToArray(), "application/pdf", "wards.pdf");
                }
            }
            catch (Exception ex)
            {
                // Log the exception and return error message
                return BadRequest("An error occurred while generating the PDF.");
            }
        }







        // Action to view deleted wards
        public async Task<IActionResult> DeletedWards(int pageNumber = 1, int pageSize = 4, string sortOrder = "")
        {
            ViewData["ShowSidebar"] = true;

            // Query to get soft-deleted wards
            IQueryable<Ward> deletedWardsQuery = _context.Wards
                .Where(w => w.WardStatus == WardStatus.Deleted)
                .Include(w => w.Beds);

            // Sorting logic
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["CapacitySortParm"] = sortOrder == "capacity" ? "capacity_desc" : "capacity";

            switch (sortOrder)
            {
                case "name_desc":
                    deletedWardsQuery = deletedWardsQuery.OrderByDescending(w => w.WardName);
                    break;
                case "capacity":
                    deletedWardsQuery = deletedWardsQuery.OrderBy(w => w.Capacity);
                    break;
                case "capacity_desc":
                    deletedWardsQuery = deletedWardsQuery.OrderByDescending(w => w.Capacity);
                    break;
                default: // Default sorting by WardName ascending
                    deletedWardsQuery = deletedWardsQuery.OrderBy(w => w.WardName);
                    break;
            }

            // Pagination
            var totalDeletedWards = await deletedWardsQuery.CountAsync();
            var totalPages = (int)Math.Ceiling(totalDeletedWards / (double)pageSize);

            var deletedWards = await deletedWardsQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewData["PageNumber"] = pageNumber;
            ViewData["TotalPages"] = totalPages;
            ViewData["CurrentSort"] = sortOrder;

            return View(deletedWards);
        }


        // Action to restore deleted wards
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreDeletedWards(List<int> selectedWards)
        {
            if (selectedWards == null || !selectedWards.Any())
            {
                TempData["ErrorMessage"] = "No wards were selected for restoration.";
                return RedirectToAction(nameof(DeletedWards));
            }

            try
            {
                foreach (var wardId in selectedWards)
                {
                    var ward = await _context.Wards
                        .Include(w => w.Beds) // Include related beds
                        .FirstOrDefaultAsync(w => w.WardId == wardId && w.WardStatus == WardStatus.Deleted);

                    if (ward != null)
                    {
                        // Restore ward status
                        ward.WardStatus = WardStatus.Active;
                        _context.Update(ward);

                        // Optionally restore beds (if necessary)
                        foreach (var bed in ward.Beds)
                        {
                            bed.DeletionStatus = CondtionStatus.Active; // Restore beds if previously marked as deleted
                            _context.Update(bed);
                        }
                    }
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Selected wards restored successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while restoring the selected wards.";
            }

            return RedirectToAction(nameof(DeletedWards));
        }








































        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkDelete(List<int> selectedBeds)
        {
            if (selectedBeds == null || !selectedBeds.Any())
            {
                TempData["ErrorMessage"] = "No beds were selected for deletion.";
                return RedirectToAction(nameof(Beds));
            }

            try
            {
                foreach (var bedId in selectedBeds)
                {
                    var bed = await _context.Beds
                        .FirstOrDefaultAsync(b => b.BedId == bedId && b.DeletionStatus == CondtionStatus.Active);
                    if (bed != null)
                    {
                        bed.DeletionStatus = CondtionStatus.Deleted;
                        _context.Update(bed);
                    }
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Selected beds deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the selected beds.";
            }

            // Store selected bed IDs for persistence
            TempData["SelectedBedIds"] = selectedBeds;

            return RedirectToAction(nameof(Beds));
        }


        [HttpGet]
        public async Task<IActionResult> GetBedNames(string term)
        {
            // Limit the number of returned results to 10
            var bedNames = await _context.Beds
                .Where(b => b.DeletionStatus == CondtionStatus.Active &&
                            (b.BedNumber.Contains(term) || b.Status.ToString().Contains(term)))
                .Select(b => new
                {
                    value = b.BedNumber, // This will be the displayed value
                    label = b.BedNumber  // This can be used for display in the suggestion box
                })
                .Take(10)  // Return only the first 10 results
                .ToListAsync();

            return Json(bedNames);
        }


        public async Task<IActionResult> Beds(int pageNumber = 1, int pageSize = 4, string sortOrder = "", string searchString = "")
        {
            ViewData["ShowSidebar"] = true;

            // Determine the sorting order
            IQueryable<Bed> bedsQuery = _context.Beds
                .Include(b => b.Ward)
                .Where(b => b.DeletionStatus == CondtionStatus.Active);

            // Apply search functionality
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                bedsQuery = bedsQuery.Where(b => b.BedNumber.ToString().Contains(searchString)  || b.Ward.WardName.Contains(searchString));
            }

            // Apply sorting based on the sortOrder parameter
            switch (sortOrder)
            {
                case "bednumber_desc":
                    bedsQuery = bedsQuery.OrderByDescending(b => b.BedNumber);
                    break;
                case "bednumber": // Ascending order for bed number
                    bedsQuery = bedsQuery.OrderBy(b => b.BedNumber);
                    break;
                case "status":
                    bedsQuery = bedsQuery.OrderBy(b => b.Status);
                    break;
                case "status_desc":
                    bedsQuery = bedsQuery.OrderByDescending(b => b.Status);
                    break;
                case "ward":
                    bedsQuery = bedsQuery.OrderBy(b => b.Ward.WardName);
                    break;
                case "ward_desc":
                    bedsQuery = bedsQuery.OrderByDescending(b => b.Ward.WardName);
                    break;
                default: // Default sort order (BedNumber ascending)
                    bedsQuery = bedsQuery.OrderBy(b => b.BedNumber);
                    break;
            }

            // Pagination logic remains the same
            var totalBeds = await bedsQuery.CountAsync();
            var totalPages = (int)Math.Ceiling(totalBeds / (double)pageSize);
            var beds = await bedsQuery.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewData["PageNumber"] = pageNumber;
            ViewData["TotalPages"] = totalPages;
            ViewData["CurrentSort"] = sortOrder;
            ViewData["CurrentSearch"] = searchString; // Store current search string for the view

            return View(beds);
        }



        public IActionResult AddBed()
        {
            ViewData["ShowSidebar"] = true;
            ViewData["WardId"] = new SelectList(_context.Wards
                .Where(w => w.WardStatus == WardStatus.Active), "WardId", "WardName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddBed([Bind("BedId,BedNumber,Status,WardId")] Bed bed)
        {
            try
            {
                var ward = await _context.Wards
                    .Include(w => w.Beds)
                    .FirstOrDefaultAsync(w => w.WardId == bed.WardId);

                if (ward == null)
                {
                    TempData["ErrorMessage"] = "Selected ward not found.";
                    return RedirectToAction(nameof(AddBed));
                }

                // Check for duplicate bed number within the same ward
                if (_context.Beds.Any(b => b.BedNumber == bed.BedNumber && b.WardId == bed.WardId && b.DeletionStatus == CondtionStatus.Active))
                {
                    TempData["ErrorMessage"] = "A bed with the same number already exists in this ward.";
                    ViewData["WardId"] = new SelectList(_context.Wards
                        .Where(w => w.WardStatus == WardStatus.Active), "WardId", "WardName", bed.WardId);
                    return View(bed);
                }

                // Check if adding the new bed would exceed the ward's capacity
                if (ward.Beds.Count >= ward.Capacity)
                {
                    TempData["ErrorMessage"] = "Cannot add bed. Ward capacity reached.";
                    ViewData["WardId"] = new SelectList(_context.Wards
                        .Where(w => w.WardStatus == WardStatus.Active), "WardId", "WardName", bed.WardId);
                    return View(bed);
                }

                // Set default deletion status and add the bed
                bed.DeletionStatus = CondtionStatus.Active;
                _context.Add(bed);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Bed added successfully.";
                return RedirectToAction(nameof(Beds));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while adding the bed: " + ex.Message;
                ViewData["WardId"] = new SelectList(_context.Wards
                    .Where(w => w.WardStatus == WardStatus.Active), "WardId", "WardName", bed.WardId);
                return View(bed);
            }
        }


        public async Task<IActionResult> EditBed(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Bed ID is required.";
                return RedirectToAction(nameof(Beds));
            }

            ViewData["ShowSidebar"] = true;

            var bed = await _context.Beds
                .Include(b => b.Ward) // Include Ward to check capacity later
                .FirstOrDefaultAsync(b => b.BedId == id);
            if (bed == null || bed.DeletionStatus == CondtionStatus.Deleted)
            {
                TempData["ErrorMessage"] = "Bed not found or has been deleted.";
                return RedirectToAction(nameof(Beds));
            }

            ViewData["WardId"] = new SelectList(_context.Wards
                .Where(w => w.WardStatus == WardStatus.Active), "WardId", "WardName", bed.WardId);

            return View(bed);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBed(int id, [Bind("BedId,BedNumber,Status,WardId,DeletionStatus")] Bed bed)
        {
            if (id != bed.BedId)
            {
                TempData["ErrorMessage"] = "Bed ID mismatch.";
                return RedirectToAction(nameof(Beds));
            }

            var existingBed = await _context.Beds
                .Include(b => b.Ward)
                .FirstOrDefaultAsync(b => b.BedId == id);

            if (existingBed == null)
            {
                TempData["ErrorMessage"] = "Bed not found.";
                return RedirectToAction(nameof(Beds));
            }

            // Check for duplicate bed number within the same ward
            if (bed.BedNumber != existingBed.BedNumber &&
                _context.Beds.Any(b => b.BedNumber == bed.BedNumber && b.WardId == bed.WardId && b.DeletionStatus == CondtionStatus.Active))
            {
                TempData["ErrorMessage"] = "A bed with the same number already exists in this ward.";
                ViewData["WardId"] = new SelectList(_context.Wards
                    .Where(w => w.WardStatus == WardStatus.Active), "WardId", "WardName", bed.WardId);
                return View(bed);
            }

            // Check if updating the bed's ward would exceed the ward's capacity
            if (bed.WardId != existingBed.WardId)
            {
                var newWard = await _context.Wards
                    .Include(w => w.Beds)
                    .FirstOrDefaultAsync(w => w.WardId == bed.WardId);

                if (newWard == null)
                {
                    TempData["ErrorMessage"] = "Selected ward not found.";
                    return RedirectToAction(nameof(EditBed), new { id });
                }

                if (newWard.Beds.Count >= newWard.Capacity)
                {
                    TempData["ErrorMessage"] = "Cannot move bed. Selected ward capacity reached.";
                    ViewData["WardId"] = new SelectList(_context.Wards
                        .Where(w => w.WardStatus == WardStatus.Active), "WardId", "WardName", bed.WardId);
                    return View(bed);
                }
            }

            // Update the properties of the existing Bed instance
            existingBed.BedNumber = bed.BedNumber;
            existingBed.Status = bed.Status;
            existingBed.WardId = bed.WardId;
            existingBed.DeletionStatus = bed.DeletionStatus;

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Bed updated successfully.";
                return RedirectToAction(nameof(Beds));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BedExists(bed.BedId))
                {
                    TempData["ErrorMessage"] = "Bed not found.";
                    return RedirectToAction(nameof(Beds));
                }
                else
                {
                    TempData["ErrorMessage"] = "An error occurred while updating the bed.";
                    throw;
                }
            }
        }

        public async Task<IActionResult> DeleteBed(int? id)
        {
            if (id == null)
            {
                TempData["BedErrorMessage"] = "Bed ID is required.";
                return RedirectToAction(nameof(Beds)); // Redirect to a relevant action with a message
            }
            ViewData["ShowSidebar"] = true;

            var bed = await _context.Beds
                .Include(b => b.Ward) // Include Ward to show relevant information if needed
                .FirstOrDefaultAsync(m => m.BedId == id && m.DeletionStatus == CondtionStatus.Active);
            if (bed == null)
            {
                TempData["BedErrorMessage"] = "Bed not found or already deleted.";
                return RedirectToAction(nameof(Beds)); // Redirect to a relevant action with a message
            }

            return View(bed); // Return the view to confirm the deletion
        }


        [HttpPost, ActionName("DeleteBed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBedConfirmed(int id)
        {
            var bed = await _context.Beds
                .FirstOrDefaultAsync(b => b.BedId == id && b.DeletionStatus == CondtionStatus.Active);

            if (bed == null)
            {
                TempData["ErrorMessage"] = "Bed not found or already deleted.";
                return RedirectToAction(nameof(Beds)); // Redirect to a relevant action with a message
            }

            // Perform soft delete
            bed.DeletionStatus = CondtionStatus.Deleted;
            _context.Update(bed);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Bed deleted successfully.";
            return RedirectToAction(nameof(Beds)); // Redirect to the list of beds
        }

        private bool BedExists(int id)
        {
            return _context.Beds.Any(e => e.BedId == id && e.DeletionStatus == CondtionStatus.Active);
        }
        public async Task<IActionResult> DeletedBeds(int pageNumber = 1, int pageSize = 4, string sortOrder = "", string searchString = "")
        {
            ViewData["ShowSidebar"] = true;

            // Query to get deleted beds
            IQueryable<Bed> deletedBedsQuery = _context.Beds
                .Where(b => b.DeletionStatus == CondtionStatus.Deleted);

            // Apply search functionality (if needed)
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                deletedBedsQuery = deletedBedsQuery.Where(b => b.BedNumber.ToString().Contains(searchString));
            }

            // Apply sorting based on the sortOrder parameter
            switch (sortOrder)
            {
                case "bednumber_desc":
                    deletedBedsQuery = deletedBedsQuery.OrderByDescending(b => b.BedNumber);
                    break;
                case "bednumber": // Ascending order for bed number
                    deletedBedsQuery = deletedBedsQuery.OrderBy(b => b.BedNumber);
                    break;
                default: // Default sort order (BedNumber ascending)
                    deletedBedsQuery = deletedBedsQuery.OrderBy(b => b.BedNumber);
                    break;
            }

            // Pagination logic
            var totalDeletedBeds = await deletedBedsQuery.CountAsync();
            var totalPages = (int)Math.Ceiling(totalDeletedBeds / (double)pageSize);
            var deletedBeds = await deletedBedsQuery.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewData["PageNumber"] = pageNumber;
            ViewData["TotalPages"] = totalPages;
            ViewData["CurrentSort"] = sortOrder;
            ViewData["CurrentSearch"] = searchString; // Store current search string for the view

            return View(deletedBeds);
        }

        public async Task<IActionResult> RestoreBed(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Bed ID is required.";
                return RedirectToAction(nameof(Beds)); // Redirect to the Beds action
            }

            var bed = await _context.Beds
                .FirstOrDefaultAsync(b => b.BedId == id && b.DeletionStatus == CondtionStatus.Deleted);

            if (bed == null)
            {
                TempData["ErrorMessage"] = "Bed not found or is not deleted.";
                return RedirectToAction(nameof(Beds)); // Redirect to the Beds action
            }

            // Restore the bed
            bed.DeletionStatus = CondtionStatus.Active;
            _context.Update(bed);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Bed restored successfully.";
            return RedirectToAction(nameof(Beds)); // Redirect to the Beds action
        }


        [HttpPost]
        public async Task<IActionResult> BulkRestore(int[] selectedBeds)
        {
            if (selectedBeds == null || selectedBeds.Length == 0)
            {
                TempData["ErrorMessage"] = "No beds selected for restoration.";
                return RedirectToAction(nameof(Beds)); // Redirect to the Beds action
            }

            var bedsToRestore = await _context.Beds
                .Where(b => selectedBeds.Contains(b.BedId) && b.DeletionStatus == CondtionStatus.Deleted)
                .ToListAsync();

            if (bedsToRestore.Count == 0)
            {
                TempData["ErrorMessage"] = "No beds found for restoration.";
                return RedirectToAction(nameof(Beds)); // Redirect to the Beds action
            }

            foreach (var bed in bedsToRestore)
            {
                bed.DeletionStatus = CondtionStatus.Active; // Restore the bed
            }

            _context.UpdateRange(bedsToRestore);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"{bedsToRestore.Count} bed(s) restored successfully.";
            return RedirectToAction(nameof(Beds)); // Redirect to the Beds action
        }



        [HttpGet]
        public async Task<IActionResult> ExportToPdf()
        {
            try
            {
                // Get hospital information
                var hospitalInfo = await _context.HospitalInfos.FirstOrDefaultAsync();

                if (hospitalInfo == null)
                {
                    return BadRequest("Hospital information is missing.");
                }

                // Get beds data, include Ward information
                var beds = await _context.Beds
                    .Where(b => b.DeletionStatus == CondtionStatus.Active)
                    .Include(b => b.Ward) // Include related Ward to access WardName
                    .ToListAsync();

                using (var memoryStream = new MemoryStream())
                {
                    var pdfWriter = new PdfWriter(memoryStream);
                    using (var pdfDocument = new PdfDocument(pdfWriter))
                    {
                        // Set document to have narrow margins
                        var document = new Document(pdfDocument, PageSize.A4);
                        document.SetMargins(20, 20, 20, 20); // Set smaller margins

                        // Add the hospital logo (reduce the size to 50 for a compact layout)
                        var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "LogoResized.png");
                        if (System.IO.File.Exists(logoPath))
                        {
                            var logoImageData = ImageDataFactory.Create(logoPath);
                            var logo = new iText.Layout.Element.Image(logoImageData)
                                .SetWidth(50) // Reduce width for better fit
                                .SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.LEFT)
                                .SetMarginBottom(10);
                            document.Add(logo);
                        }

                        // Add hospital info at the top left corner (reduce font size for compact layout)
                        var hospitalInfoParagraph = new Paragraph($"{hospitalInfo.HospitalName}\n{hospitalInfo.Address}\n{hospitalInfo.PhoneNumber}")
                            .SetFontSize(8) // Reduced font size
                            .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
                            .SetMarginBottom(10);
                        document.Add(hospitalInfoParagraph);

                        // Add a header (reduce header font size)
                        document.Add(new Paragraph("Beds Data Export")
                            .SetBold()
                            .SetFontSize(16) // Slightly smaller header font size
                            .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                            .SetMarginBottom(10));

                        // Create a table with 4 columns (reduce the table size)
                        var table = new Table(UnitValue.CreatePercentArray(new float[] { 1, 2, 2, 2 })) // Set column widths
                            .UseAllAvailableWidth(); // Make table use the full width of the page

                        // Set table headers with "Ward Name" instead of "Ward ID"
                        table.AddHeaderCell(new Cell().Add(new Paragraph("Bed ID").SetFontSize(8))); // Reduce font size
                        table.AddHeaderCell(new Cell().Add(new Paragraph("Bed Number").SetFontSize(8)));
                        table.AddHeaderCell(new Cell().Add(new Paragraph("Status").SetFontSize(8)));
                        table.AddHeaderCell(new Cell().Add(new Paragraph("Ward Name").SetFontSize(8))); // Changed from "Ward ID"

                        // Populate the table with beds data
                        foreach (var bed in beds)
                        {
                            table.AddCell(new Cell().Add(new Paragraph(bed.BedId.ToString()).SetFontSize(8))); // Reduced font size
                            table.AddCell(new Cell().Add(new Paragraph(bed.BedNumber).SetFontSize(8)));
                            table.AddCell(new Cell().Add(new Paragraph(bed.Status.ToString()).SetFontSize(8)));
                            table.AddCell(new Cell().Add(new Paragraph(bed.Ward.WardName).SetFontSize(8))); // Show WardName instead of WardId
                        }

                        // Add the table to the document
                        document.Add(table);

                        // Summary Section (reduce font size and margins)
                        document.Add(new Paragraph("\nSummary:")
                            .SetBold()
                            .SetFontSize(10) // Smaller summary font size
                            .SetMarginTop(10));

                        document.Add(new Paragraph($"Total Beds: {beds.Count}")
                            .SetFontSize(8)); // Reduced font size

                        document.Add(new Paragraph($"Generated on: {DateTime.Now.ToString("MM/dd/yyyy HH:mm")}")
                            .SetFontSize(8)); // Reduced font size

                        document.Close();
                    }

                    return File(memoryStream.ToArray(), "application/pdf", "Beds.pdf");
                }
            }
            catch (Exception ex)
            {
                // Log the exception (use your logging framework)
                // Return an appropriate error view/message
                return BadRequest("An error occurred while generating the PDF.");
            }
        }

































        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkDeleteConsumable(int[] selectedConsumables)
        {
            if (selectedConsumables == null || !selectedConsumables.Any())
            {
                TempData["ErrorMessage"] = "No consumables were selected for deletion.";
                return RedirectToAction(nameof(Consumables));
            }

            var consumablesToDelete = await _context.Consumables
                .Where(c => selectedConsumables.Contains(c.ConsumableId) && c.DeletionStatus == ConsumableStatus.Active)
                .ToListAsync();

            if (!consumablesToDelete.Any())
            {
                TempData["ErrorMessage"] = "No consumables found for deletion.";
                return RedirectToAction(nameof(Consumables));
            }

            foreach (var consumable in consumablesToDelete)
            {
                consumable.DeletionStatus = ConsumableStatus.Deleted; // Soft delete
                _context.Update(consumable);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Selected consumables deleted successfully!";
            return RedirectToAction(nameof(Consumables));
        }





        public async Task<IActionResult> Consumables(int pageNumber = 1, int pageSize = 4, string sortOrder = "", string searchString = "")
        {
            ViewData["ShowSidebar"] = true;

            // Determine the sorting order
            IQueryable<Consumable> consumablesQuery = _context.Consumables
                .Where(c => c.DeletionStatus == ConsumableStatus.Active);

            // Apply search functionality
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                consumablesQuery = consumablesQuery.Where(c =>
                    c.Name.Contains(searchString) ||
                    c.Description.Contains(searchString));
            }

            // Apply sorting based on the sortOrder parameter
            switch (sortOrder)
            {
                case "name_desc":
                    consumablesQuery = consumablesQuery.OrderByDescending(c => c.Name);
                    break;
                case "type":
                    consumablesQuery = consumablesQuery.OrderBy(c => c.Type);
                    break;
                case "type_desc":
                    consumablesQuery = consumablesQuery.OrderByDescending(c => c.Type);
                    break;
                case "expiry_date":
                    consumablesQuery = consumablesQuery.OrderBy(c => c.ExpiryDate);
                    break;
                case "expiry_date_desc":
                    consumablesQuery = consumablesQuery.OrderByDescending(c => c.ExpiryDate);
                    break;
                default: // Name ascending 
                    consumablesQuery = consumablesQuery.OrderBy(c => c.Name);
                    break;
            }

            // Pagination
            var totalConsumables = await consumablesQuery.CountAsync();
            var totalPages = (int)Math.Ceiling(totalConsumables / (double)pageSize);

            var consumables = await consumablesQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewData["PageNumber"] = pageNumber;
            ViewData["TotalPages"] = totalPages;
            ViewData["CurrentSort"] = sortOrder;
            ViewData["CurrentSearch"] = searchString; // Store current search string for the view

            return View(consumables);
        }

        [HttpGet]
        public async Task<IActionResult> GetConsumableNames(string term)
        {
            var consumableNames = await _context.Consumables
                .Where(c => c.DeletionStatus == ConsumableStatus.Active && c.Name.Contains(term))
                .Select(c => c.Name)
                .ToListAsync();

            return Json(consumableNames);
        }


        public IActionResult AddConsumable()
        {
            ViewData["ShowSidebar"] = true;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddConsumable([Bind("ConsumableId,Name,Type,Quantity,ExpiryDate,Description")] Consumable consumable)
        {
            // Check for existing consumable with the same name and active status
            bool exists = await _context.Consumables
                .AnyAsync(c => c.Name.ToLower() == consumable.Name.ToLower() && c.DeletionStatus == ConsumableStatus.Active);

            if (exists)
            {
                TempData["ErrorMessage"] = "A consumable with this name already exists. Please use a different name.";
                return View(consumable);
            }

            consumable.DeletionStatus = ConsumableStatus.Active; // Set default status
            consumable.CreatedDate = DateTime.Now;
            consumable.LastUpdatedDate = DateTime.Now;
            _context.Add(consumable);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Consumable added successfully!";
            return RedirectToAction(nameof(Consumables));

            TempData["ErrorMessage"] = "Failed to add consumable. Please check the input and try again.";
            return View(consumable);
        }


        public async Task<IActionResult> EditConsumable(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewData["ShowSidebar"] = true;

            var consumable = await _context.Consumables.FindAsync(id);
            if (consumable == null || consumable.DeletionStatus == ConsumableStatus.Deleted)
            {
                return NotFound();
            }
            return View(consumable);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditConsumable(int id, [Bind("ConsumableId,Name,Type,Quantity,ExpiryDate,Description,DeletionStatus")] Consumable consumable)
        {
            if (id != consumable.ConsumableId)
            {
                return NotFound();
            }

           
                try
                {
                    consumable.LastUpdatedDate = DateTime.Now;
                    _context.Update(consumable);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Consumable updated successfully!";
                    return RedirectToAction(nameof(Consumables));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ConsumableExists(consumable.ConsumableId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Error updating consumable. Please try again.";
                        throw;
                    }
                }
            

            TempData["ErrorMessage"] = "Failed to update consumable. Please check the input and try again.";
            return View(consumable);
        }

        public async Task<IActionResult> DeleteConsumable(int? id)
        {
            ViewData["ShowSidebar"] = true;
            if (id == null)
            {
                return NotFound();
            }

            var consumable = await _context.Consumables
                .FirstOrDefaultAsync(m => m.ConsumableId == id && m.DeletionStatus == ConsumableStatus.Active);
            if (consumable == null)
            {
                return NotFound();
            }

            return View(consumable);
        }

        [HttpPost, ActionName("DeleteConsumable")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConsumableConfirmed(int id)
        {
            var consumable = await _context.Consumables.FindAsync(id);

            if (consumable == null || consumable.DeletionStatus == ConsumableStatus.Deleted)
            {
                return NotFound();
            }

            // Soft delete consumable
            consumable.DeletionStatus = ConsumableStatus.Deleted;
            _context.Update(consumable);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Consumable deleted successfully!";
            return RedirectToAction(nameof(Consumables));
        }
        private bool ConsumableExists(int id)
        {
            return _context.Consumables.Any(e => e.ConsumableId == id && e.DeletionStatus == ConsumableStatus.Active);
        }


        public async Task<IActionResult> DeletedConsumables(int pageNumber = 1, int pageSize = 4, string sortOrder = "", string searchString = "")
        {
            ViewData["ShowSidebar"] = true;

            // Determine the sorting order for deleted consumables
            IQueryable<Consumable> deletedConsumablesQuery = _context.Consumables
                .Where(c => c.DeletionStatus == ConsumableStatus.Deleted);

            // Apply search functionality for deleted consumables
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                deletedConsumablesQuery = deletedConsumablesQuery.Where(c =>
                    c.Name.Contains(searchString) ||
                    c.Description.Contains(searchString));
            }

            // Apply sorting based on the sortOrder parameter
            switch (sortOrder)
            {
                case "name_desc":
                    deletedConsumablesQuery = deletedConsumablesQuery.OrderByDescending(c => c.Name);
                    break;
                case "type":
                    deletedConsumablesQuery = deletedConsumablesQuery.OrderBy(c => c.Type);
                    break;
                case "type_desc":
                    deletedConsumablesQuery = deletedConsumablesQuery.OrderByDescending(c => c.Type);
                    break;
                case "expiry_date":
                    deletedConsumablesQuery = deletedConsumablesQuery.OrderBy(c => c.ExpiryDate);
                    break;
                case "expiry_date_desc":
                    deletedConsumablesQuery = deletedConsumablesQuery.OrderByDescending(c => c.ExpiryDate);
                    break;
                default: // Name ascending 
                    deletedConsumablesQuery = deletedConsumablesQuery.OrderBy(c => c.Name);
                    break;
            }

            // Pagination
            var totalDeletedConsumables = await deletedConsumablesQuery.CountAsync();
            var totalPages = (int)Math.Ceiling(totalDeletedConsumables / (double)pageSize);

            var deletedConsumables = await deletedConsumablesQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewData["PageNumber"] = pageNumber;
            ViewData["TotalPages"] = totalPages;
            ViewData["CurrentSort"] = sortOrder;
            ViewData["CurrentSearch"] = searchString; // Store current search string for the view

            return View(deletedConsumables);
        }
        public async Task<IActionResult> DeletedConsumables()
        {
            var deletedConsumables = await _context.Consumables
                .Where(c => c.DeletionStatus == ConsumableStatus.Deleted)
                .ToListAsync();

            return View(deletedConsumables);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreConsumable(int id)
        {
            var consumable = await _context.Consumables.FindAsync(id);

            if (consumable == null || consumable.DeletionStatus != ConsumableStatus.Deleted)
            {
                return NotFound();
            }

            // Restore the consumable
            consumable.DeletionStatus = ConsumableStatus.Active;
            _context.Update(consumable);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Consumable restored successfully!";
            return RedirectToAction(nameof(DeletedConsumables));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreConsumables(List<int> consumableIds)
        {
            if (consumableIds == null || !consumableIds.Any())
            {
                TempData["ErrorMessage"] = "No consumables selected for restoration.";
                return RedirectToAction(nameof(DeletedConsumables));
            }

            var consumablesToRestore = await _context.Consumables
                .Where(c => consumableIds.Contains(c.ConsumableId) && c.DeletionStatus == ConsumableStatus.Deleted)
                .ToListAsync();

            foreach (var consumable in consumablesToRestore)
            {
                consumable.DeletionStatus = ConsumableStatus.Active;
            }

            _context.UpdateRange(consumablesToRestore);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Selected consumables restored successfully!";
            return RedirectToAction(nameof(DeletedConsumables));
        }

        [HttpGet]
        public async Task<IActionResult> ExportConsumablesToPdf(string searchString = "")
        {
            try
            {
                // Get hospital information
                var hospitalInfo = await _context.HospitalInfos.FirstOrDefaultAsync();

                if (hospitalInfo == null)
                {
                    return BadRequest("Hospital information is missing.");
                }

                // Fetch consumables based on search criteria
                var consumablesQuery = _context.Consumables.Where(c => c.DeletionStatus == ConsumableStatus.Active);

                if (!string.IsNullOrWhiteSpace(searchString))
                {
                    consumablesQuery = consumablesQuery.Where(c =>
                        c.Name.Contains(searchString) ||
                        c.Description.Contains(searchString));
                }

                var consumables = await consumablesQuery.ToListAsync();

                using (var memoryStream = new MemoryStream())
                {
                    var pdfWriter = new PdfWriter(memoryStream);
                    using (var pdfDocument = new PdfDocument(pdfWriter))
                    {
                        // Set document to have narrow margins
                        var document = new Document(pdfDocument, PageSize.A4);
                        document.SetMargins(20, 20, 20, 20); // Set smaller margins

                        // Add the hospital logo (reduce the size to 50 for a compact layout)
                        var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "LogoResized.png");
                        if (System.IO.File.Exists(logoPath))
                        {
                            var logoImageData = ImageDataFactory.Create(logoPath);
                            var logo = new iText.Layout.Element.Image(logoImageData)
                                .SetWidth(50) // Reduce width for better fit
                                .SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.LEFT)
                                .SetMarginBottom(10);
                            document.Add(logo);
                        }

                        // Add hospital info at the top left corner (reduce font size for compact layout)
                        var hospitalInfoParagraph = new Paragraph($"{hospitalInfo.HospitalName}\n{hospitalInfo.Address}\n{hospitalInfo.PhoneNumber}")
                            .SetFontSize(8) // Reduced font size
                            .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
                            .SetMarginBottom(10);
                        document.Add(hospitalInfoParagraph);

                        // Add a header (reduce header font size)
                        document.Add(new Paragraph("Consumables Data Export")
                            .SetBold()
                            .SetFontSize(16) // Slightly smaller header font size
                            .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                            .SetMarginBottom(10));

                        // Create a table with 5 columns for consumables data
                        var table = new Table(UnitValue.CreatePercentArray(new float[] { 1, 2, 2, 2, 3 })) // Set column widths
                            .UseAllAvailableWidth(); // Make table use the full width of the page

                        // Set table headers
                        table.AddHeaderCell(new Cell().Add(new Paragraph("Name").SetFontSize(8))); // Reduce font size
                        table.AddHeaderCell(new Cell().Add(new Paragraph("Type").SetFontSize(8)));
                        table.AddHeaderCell(new Cell().Add(new Paragraph("Quantity").SetFontSize(8)));
                        table.AddHeaderCell(new Cell().Add(new Paragraph("Expiry Date").SetFontSize(8)));
                        table.AddHeaderCell(new Cell().Add(new Paragraph("Description").SetFontSize(8)));

                        // If ConsumableType is an enum
                        foreach (var consumable in consumables)
                        {
                            table.AddCell(new Cell().Add(new Paragraph(consumable.Name).SetFontSize(8)));

                            // Use ToString() for enum
                            table.AddCell(new Cell().Add(new Paragraph(consumable.Type.ToString()).SetFontSize(8)));

                            table.AddCell(new Cell().Add(new Paragraph(consumable.Quantity.ToString()).SetFontSize(8)));
                            table.AddCell(new Cell().Add(new Paragraph(consumable.ExpiryDate.ToShortDateString()).SetFontSize(8)));
                            table.AddCell(new Cell().Add(new Paragraph(consumable.Description).SetFontSize(8)));
                        }



                        // Add the table to the document
                        document.Add(table);

                        // Summary Section (reduce font size and margins)
                        document.Add(new Paragraph("\nSummary:")
                            .SetBold()
                            .SetFontSize(10) // Smaller summary font size
                            .SetMarginTop(10));

                        document.Add(new Paragraph($"Total Consumables: {consumables.Count}")
                            .SetFontSize(8)); // Reduced font size

                        document.Add(new Paragraph($"Generated on: {DateTime.Now.ToString("MM/dd/yyyy HH:mm")}")
                            .SetFontSize(8)); // Reduced font size

                        document.Close();
                    }

                    return File(memoryStream.ToArray(), "application/pdf", "Consumables.pdf");
                }
            }
            catch (Exception ex)
            {
                // Log the exception (use your logging framework)
                // Return an appropriate error view/message
                return BadRequest("An error occurred while generating the PDF.");
            }
        }










































        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkDeleteMedication(int[] selectedMedications)
        {
            if (selectedMedications == null || !selectedMedications.Any())
            {
                TempData["ErrorMessage"] = "No medications were selected for deletion.";
                return RedirectToAction(nameof(Medications));
            }

            var medicationsToDelete = await _context.Medications
                .Where(m => selectedMedications.Contains(m.MedicationId) && m.DeletionStatus == MedicationStatus.Active)
                .ToListAsync();

            if (!medicationsToDelete.Any())
            {
                TempData["ErrorMessage"] = "No medications found for deletion.";
                return RedirectToAction(nameof(Medications));
            }

            foreach (var medication in medicationsToDelete)
            {
                medication.DeletionStatus = MedicationStatus.Deleted; // Soft delete
                _context.Update(medication);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Selected medications deleted successfully!";
            return RedirectToAction(nameof(Medications));
        }


        [HttpGet]
        public async Task<IActionResult> GetMedicationNames(string term)
        {
            var medicationNames = await _context.Medications
                .Where(m => m.DeletionStatus == MedicationStatus.Active && m.Name.Contains(term))
                .Select(m => m.Name)
                .ToListAsync();

            return Json(medicationNames);
        }





        public async Task<IActionResult> Medications(int pageNumber = 1, int pageSize = 4, string searchString = "")
        {
            ViewData["ShowSidebar"] = true;

            // Fetch medications from the database, filtering by active status
            var medicationsQuery = _context.Medications
                .Where(m => m.DeletionStatus == MedicationStatus.Active);

            // Apply search functionality
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                medicationsQuery = medicationsQuery
                    .Where(m => m.Name.Contains(searchString) || m.Description.Contains(searchString));
            }

            // Sorting alphabetically by name
            medicationsQuery = medicationsQuery.OrderBy(m => m.Name);

            // Pagination
            var totalMedications = await medicationsQuery.CountAsync();
            var totalPages = (int)Math.Ceiling(totalMedications / (double)pageSize);

            var medications = await medicationsQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Pass pagination data and search string to the view
            ViewData["PageNumber"] = pageNumber;
            ViewData["TotalPages"] = totalPages;
            ViewData["SearchString"] = searchString;

            return View(medications);
        }



        public IActionResult AddMedication()
        {
            ViewData["ShowSidebar"] = true;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMedication([Bind("MedicationId,Name,Type,Quantity,ExpiryDate,Description")] Medication medication)
        {
            if (ModelState.IsValid)
            {
                // Check for existing medication with the same name and active status
                bool exists = await _context.Medications
                    .AnyAsync(m => m.Name.Equals(medication.Name, StringComparison.OrdinalIgnoreCase) && m.DeletionStatus == MedicationStatus.Active);

                if (exists)
                {
                    TempData["ErrorMessage"] = "A medication with this name already exists. Please use a different name.";
                    return View(medication);
                }

                medication.DeletionStatus = MedicationStatus.Active; // Set default status
                _context.Add(medication);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Medication added successfully!";
                return RedirectToAction(nameof(Medications));
            }

            TempData["ErrorMessage"] = "Failed to add medication. Please check the input and try again.";
            return View(medication);
        }


        public async Task<IActionResult> EditMedication(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewData["ShowSidebar"] = true;

            var medication = await _context.Medications.FindAsync(id);
            if (medication == null || medication.DeletionStatus == MedicationStatus.Deleted)
            {
                return NotFound();
            }
            return View(medication);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMedication(int id, [Bind("MedicationId,Name,Type,Quantity,ExpiryDate,Description,DeletionStatus")] Medication medication)
        {
            if (id != medication.MedicationId)
            {
                return NotFound();
            }

            // Check for duplication
            var existingMedication = await _context.Medications
                .FirstOrDefaultAsync(m => m.Name == medication.Name && m.DeletionStatus == MedicationStatus.Active && m.MedicationId != id);

            if (existingMedication != null)
            {
                ModelState.AddModelError("Name", "A medication with this name already exists.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(medication);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Medication updated successfully!";
                    return RedirectToAction(nameof(Medications));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MedicationExists(medication.MedicationId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Error updating medication. Please try again.";
                        throw;
                    }
                }
            }

            TempData["ErrorMessage"] = "Failed to update medication. Please check the input and try again.";
            return View(medication);
        }

        public async Task<IActionResult> DeleteMedication(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewData["ShowSidebar"] = true;

            var medication = await _context.Medications
                .FirstOrDefaultAsync(m => m.MedicationId == id && m.DeletionStatus == MedicationStatus.Active);
            if (medication == null)
            {
                return NotFound();
            }

            return View(medication);
        }

        [HttpPost, ActionName("DeleteMedication")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMedicationConfirmed(int id)
        {
            var medication = await _context.Medications.FindAsync(id);

            if (medication == null || medication.DeletionStatus == MedicationStatus.Deleted)
            {
                return NotFound();
            }

            // Soft delete medication
            medication.DeletionStatus = MedicationStatus.Deleted;
            _context.Update(medication);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Medication deleted successfully!";
            return RedirectToAction(nameof(Medications));
        }
        private bool MedicationExists(int id)
        {
            return _context.Medications.Any(e => e.MedicationId == id && e.DeletionStatus == MedicationStatus.Active);
        }
        [HttpGet]
        public async Task<IActionResult> ExportMedicationsToPdf()
        {
            try
            {
                // Get hospital information
                var hospitalInfo = await _context.HospitalInfos.FirstOrDefaultAsync();

                if (hospitalInfo == null)
                {
                    return BadRequest("Hospital information is missing.");
                }

                // Retrieve medications from the database
                var medications = await _context.Medications
                    .Where(m => m.DeletionStatus == MedicationStatus.Active)
                    .ToListAsync();

                using (var memoryStream = new MemoryStream())
                {
                    // Create PDF document
                    var pdfWriter = new PdfWriter(memoryStream);
                    using (var pdfDocument = new PdfDocument(pdfWriter))
                    {
                        // Set document to have narrow margins
                        var document = new Document(pdfDocument, PageSize.A4);
                        document.SetMargins(20, 20, 20, 20); // Set smaller margins

                        // Add the hospital logo (reduce the size to 50 for a compact layout)
                        var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "LogoResized.png");
                        if (System.IO.File.Exists(logoPath))
                        {
                            var logoImageData = ImageDataFactory.Create(logoPath);
                            var logo = new iText.Layout.Element.Image(logoImageData)
                                .SetWidth(50) // Reduce width for better fit
                                .SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.LEFT)
                                .SetMarginBottom(10);
                            document.Add(logo);
                        }

                        // Add hospital info at the top left corner (reduce font size for compact layout)
                        var hospitalInfoParagraph = new Paragraph($"{hospitalInfo.HospitalName}\n{hospitalInfo.Address}\n{hospitalInfo.PhoneNumber}")
                            .SetFontSize(8) // Reduced font size
                            .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
                            .SetMarginBottom(10);
                        document.Add(hospitalInfoParagraph);

                        // Add title
                        document.Add(new Paragraph("Medications Report")
                            .SetFontSize(16)
                            .SetBold()
                            .SetTextAlignment(TextAlignment.CENTER)
                            .SetMarginBottom(10));

                        // Create table
                        var table = new Table(UnitValue.CreatePercentArray(new float[] { 1, 3, 2, 2, 2, 4 }))
                            .SetWidth(UnitValue.CreatePercentValue(100));

                        // Add header
                        table.AddHeaderCell(new Cell().Add(new Paragraph("Medication ID").SetFontSize(8)));
                        table.AddHeaderCell(new Cell().Add(new Paragraph("Name").SetFontSize(8)));
                        table.AddHeaderCell(new Cell().Add(new Paragraph("Type").SetFontSize(8)));
                        table.AddHeaderCell(new Cell().Add(new Paragraph("Quantity").SetFontSize(8)));
                        table.AddHeaderCell(new Cell().Add(new Paragraph("Expiry Date").SetFontSize(8)));
                        table.AddHeaderCell(new Cell().Add(new Paragraph("Description").SetFontSize(8)));

                        // Add data to the table
                        foreach (var medication in medications)
                        {
                            table.AddCell(new Cell().Add(new Paragraph(medication.MedicationId.ToString()).SetFontSize(8)));
                            table.AddCell(new Cell().Add(new Paragraph(medication.Name).SetFontSize(8)));
                            table.AddCell(new Cell().Add(new Paragraph(medication.Type.ToString()).SetFontSize(8)));
                            table.AddCell(new Cell().Add(new Paragraph(medication.Quantity.ToString()).SetFontSize(8)));
                            table.AddCell(new Cell().Add(new Paragraph(medication.ExpiryDate.ToShortDateString()).SetFontSize(8)));
                            table.AddCell(new Cell().Add(new Paragraph(medication.Description).SetFontSize(8)));
                        }

                        // Add the table to the document
                        document.Add(table);

                        // Summary Section (reduce font size and margins)
                        document.Add(new Paragraph("\nSummary:")
                            .SetBold()
                            .SetFontSize(10)
                            .SetMarginTop(10));

                        document.Add(new Paragraph($"Total Medications: {medications.Count}")
                            .SetFontSize(8)); // Reduced font size

                        document.Add(new Paragraph($"Generated on: {DateTime.Now.ToString("MM/dd/yyyy HH:mm")}")
                            .SetFontSize(8)); // Reduced font size

                        document.Close();
                    }

                    // Return the PDF file
                    var pdfBytes = memoryStream.ToArray();
                    return File(pdfBytes, "application/pdf", "Medications.pdf");
                }
            }
            catch (Exception ex)
            {
                // Log the exception (use your logging framework)
                // Return an appropriate error view/message
                return BadRequest("An error occurred while generating the PDF.");
            }
        }


        public async Task<IActionResult> DeletedMedications(int pageNumber = 1, int pageSize = 4, string searchString = "")
        {
            ViewData["ShowSidebar"] = true;

            // Fetch deleted medications from the database
            var deletedMedicationsQuery = _context.Medications
                .Where(m => m.DeletionStatus == MedicationStatus.Deleted);

            // Apply search functionality
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                deletedMedicationsQuery = deletedMedicationsQuery
                    .Where(m => m.Name.Contains(searchString) || m.Description.Contains(searchString));
            }

            // Sorting alphabetically by name
            deletedMedicationsQuery = deletedMedicationsQuery.OrderBy(m => m.Name);

            // Pagination
            var totalDeletedMedications = await deletedMedicationsQuery.CountAsync();
            var totalPages = (int)Math.Ceiling(totalDeletedMedications / (double)pageSize);

            var deletedMedications = await deletedMedicationsQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Pass pagination data and search string to the view
            ViewData["PageNumber"] = pageNumber;
            ViewData["TotalPages"] = totalPages;
            ViewData["SearchString"] = searchString;

            return View(deletedMedications);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreMedication(int id)
        {
            var medication = await _context.Medications.FindAsync(id);

            if (medication == null || medication.DeletionStatus == MedicationStatus.Active)
            {
                return NotFound();
            }

            // Restore medication by setting DeletionStatus back to Active
            medication.DeletionStatus = MedicationStatus.Active;
            _context.Update(medication);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Medication restored successfully!";
            return RedirectToAction(nameof(DeletedMedications));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkRestoreMedications(int[] selectedMedications)
        {
            if (selectedMedications == null || !selectedMedications.Any())
            {
                TempData["ErrorMessage"] = "No medications were selected for restoration.";
                return RedirectToAction(nameof(DeletedMedications));
            }

            var medicationsToRestore = await _context.Medications
                .Where(m => selectedMedications.Contains(m.MedicationId) && m.DeletionStatus == MedicationStatus.Deleted)
                .ToListAsync();

            if (!medicationsToRestore.Any())
            {
                TempData["ErrorMessage"] = "No medications found for restoration.";
                return RedirectToAction(nameof(DeletedMedications));
            }

            foreach (var medication in medicationsToRestore)
            {
                medication.DeletionStatus = MedicationStatus.Active; // Restore
                _context.Update(medication);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Selected medications restored successfully!";
            return RedirectToAction(nameof(DeletedMedications));
        }















































        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkDeleteAllergies(string selectedIds)
        {
            if (string.IsNullOrWhiteSpace(selectedIds))
            {
                TempData["ErrorMessage"] = "No allergies selected for deletion.";
                return RedirectToAction(nameof(Allergies));
            }



            // Convert the string of selected IDs to an integer array
            var selectedAllergyIds = selectedIds.Split(',').Select(int.Parse).ToArray();

            // Fetch allergies from the database that are active and match the selected IDs
            var allergiesToDelete = await _context.Allergies
                .Where(a => selectedAllergyIds.Contains(a.AllergyId) && a.DeletionStatus == AllergyStatus.Active)
                .ToListAsync();

            // Mark these allergies as deleted
            foreach (var allergy in allergiesToDelete)
            {
                allergy.DeletionStatus = AllergyStatus.Deleted;
                _context.Update(allergy);
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Selected allergies deleted successfully!";
            return RedirectToAction(nameof(Allergies));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllergyNames(string term)
        {
            // Limit the number of returned results to 10
            var allergyNames = await _context.Allergies
                .Where(a => a.DeletionStatus == AllergyStatus.Active && a.Name.Contains(term))
                .OrderBy(a => a.Name)  // Optional: Order by name to ensure consistent results
                .Take(10)  // Return only the first 10 results
                .Select(a => a.Name)
                .ToListAsync();

            return Json(allergyNames);
        }




        [HttpGet("Allergies")]
        public async Task<IActionResult> Allergies(int pageNumber = 1, int pageSize = 4, List<int> selectedIds = null, string searchString = "")
        {
            ViewData["ShowSidebar"] = true;
            var query = _context.Allergies.AsQueryable();

            // Apply search functionality
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(a => a.Name.Contains(searchString) || a.Description.Contains(searchString));
            }

            // Retrieve all allergies without filtering by status
            var totalAllergies = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalAllergies / (double)pageSize);

            var allergies = await query
                .OrderBy(a => a.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Store the selected IDs in ViewData for the view
            ViewData["SelectedIds"] = selectedIds ?? new List<int>();

            // Store the search string in ViewData for the view
            ViewData["SearchString"] = searchString;

            ViewData["PageNumber"] = pageNumber;
            ViewData["TotalPages"] = totalPages;

            return View(allergies);
        }





        [HttpGet]
        public IActionResult AddAllergy()
        {
            ViewData["ShowSidebar"] = true;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAllergy([Bind("AllergyId,Name,Description")] Allergy allergy)
        {
            if (ModelState.IsValid)
            {
                // Convert name to lowercase for case-insensitive comparison
                string allergyNameLower = allergy.Name.ToLower();

                // Check for existing allergy with the same name and active status
                bool exists = await _context.Allergies
                    .AnyAsync(a => a.Name.ToLower() == allergyNameLower && a.DeletionStatus == AllergyStatus.Active);

                if (exists)
                {
                    TempData["ErrorMessage"] = "An allergy with this name already exists. Please use a different name.";
                    return View(allergy);
                }

                allergy.DeletionStatus = AllergyStatus.Active; // Set default status
                _context.Add(allergy);
                await _context.SaveChangesAsync();

                // Store the ID of the newly added allergy in TempData
                TempData["NewlyAddedAllergyId"] = allergy.AllergyId;

                TempData["SuccessMessage"] = "Allergy added successfully!";
                return RedirectToAction(nameof(Allergies));
            }

            TempData["ErrorMessage"] = "Failed to add allergy. Please check the input and try again.";
            return View(allergy);
        }


        public async Task<IActionResult> EditAllergy(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewData["ShowSidebar"] = true;

            var allergy = await _context.Allergies.FindAsync(id);
            if (allergy == null || allergy.DeletionStatus == AllergyStatus.Deleted)
            {
                return NotFound();
            }
            return View(allergy);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAllergy(int id, [Bind("AllergyId,Name,Description,DeletionStatus")] Allergy allergy)
        {
            if (id != allergy.AllergyId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(allergy);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Allergy updated successfully!";
                    return RedirectToAction(nameof(Allergies));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AllergyExists(allergy.AllergyId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Error updating allergy. Please try again.";
                        throw;
                    }
                }
            }

            TempData["ErrorMessage"] = "Failed to update allergy. Please check the input and try again.";
            return View(allergy);
        }

        public async Task<IActionResult> DeleteAllergy(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewData["ShowSidebar"] = true;

            var allergy = await _context.Allergies
                .FirstOrDefaultAsync(a => a.AllergyId == id && a.DeletionStatus == AllergyStatus.Active);
            if (allergy == null)
            {
                return NotFound();
            }

            return View(allergy);
        }

        [HttpPost, ActionName("DeleteAllergy")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAllergyConfirmed(int id)
        {
            var allergy = await _context.Allergies.FindAsync(id);

            if (allergy == null || allergy.DeletionStatus == AllergyStatus.Deleted)
            {
                return NotFound();
            }

            // Soft delete allergy
            allergy.DeletionStatus = AllergyStatus.Deleted;
            _context.Update(allergy);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Allergy deleted successfully!";
            return RedirectToAction(nameof(Allergies));
        }

        private bool AllergyExists(int id)
        {
            return _context.Allergies.Any(e => e.AllergyId == id && e.DeletionStatus == AllergyStatus.Active);
        }



        [HttpGet("DeletedAllergies")]
        public async Task<IActionResult> DeletedAllergies(int pageNumber = 1, int pageSize = 4)
        {
            ViewData["ShowSidebar"] = true;

            var deletedAllergies = await _context.Allergies
                .Where(a => a.DeletionStatus == AllergyStatus.Deleted)
                .OrderBy(a => a.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Total count for pagination
            var totalDeletedAllergies = await _context.Allergies.CountAsync(a => a.DeletionStatus == AllergyStatus.Deleted);
            var totalPages = (int)Math.Ceiling(totalDeletedAllergies / (double)pageSize);

            ViewData["PageNumber"] = pageNumber;
            ViewData["TotalPages"] = totalPages;

            return View(deletedAllergies);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreAllergy(int id)
        {
            var allergy = await _context.Allergies.FindAsync(id);

            if (allergy == null || allergy.DeletionStatus == AllergyStatus.Active)
            {
                return NotFound();
            }

            // Restore the deleted allergy
            allergy.DeletionStatus = AllergyStatus.Active;
            _context.Update(allergy);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Allergy restored successfully!";
            return RedirectToAction(nameof(DeletedAllergies));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkRestoreAllergies(string selectedIds)
        {
            if (string.IsNullOrWhiteSpace(selectedIds))
            {
                TempData["ErrorMessage"] = "No allergies selected for restoration.";
                return RedirectToAction(nameof(DeletedAllergies));
            }

            // Convert the string of selected IDs to an integer array
            var selectedAllergyIds = selectedIds.Split(',').Select(int.Parse).ToArray();

            // Fetch allergies from the database that are deleted and match the selected IDs
            var allergiesToRestore = await _context.Allergies
                .Where(a => selectedAllergyIds.Contains(a.AllergyId) && a.DeletionStatus == AllergyStatus.Deleted)
                .ToListAsync();

            // Mark these allergies as active
            foreach (var allergy in allergiesToRestore)
            {
                allergy.DeletionStatus = AllergyStatus.Active;
                _context.Update(allergy);
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Selected allergies restored successfully!";
            return RedirectToAction(nameof(DeletedAllergies));
        }

        [HttpGet]
        public async Task<IActionResult> ExportAllergiesToPdf()
        {
            try
            {
                // Get hospital information
                var hospitalInfo = await _context.HospitalInfos.FirstOrDefaultAsync();

                if (hospitalInfo == null)
                {
                    return BadRequest("Hospital information is missing.");
                }

                // Retrieve allergies from the database
                var allergies = await _context.Allergies
                    .Where(a => a.DeletionStatus == AllergyStatus.Active)
                    .ToListAsync();

                using (var memoryStream = new MemoryStream())
                {
                    // Create PDF document
                    var pdfWriter = new PdfWriter(memoryStream);
                    using (var pdfDocument = new PdfDocument(pdfWriter))
                    {
                        // Set document to have narrow margins
                        var document = new Document(pdfDocument, PageSize.A4);
                        document.SetMargins(20, 20, 20, 20); // Set smaller margins

                        // Add the hospital logo (reduce the size to 50 for a compact layout)
                        var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "LogoResized.png");
                        if (System.IO.File.Exists(logoPath))
                        {
                            var logoImageData = ImageDataFactory.Create(logoPath);
                            var logo = new iText.Layout.Element.Image(logoImageData)
                                .SetWidth(50) // Reduce width for better fit
                                .SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.LEFT)
                                .SetMarginBottom(10);
                            document.Add(logo);
                        }

                        // Add hospital info at the top left corner (reduce font size for compact layout)
                        var hospitalInfoParagraph = new Paragraph($"{hospitalInfo.HospitalName}\n{hospitalInfo.Address}\n{hospitalInfo.PhoneNumber}")
                            .SetFontSize(8) // Reduced font size
                            .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
                            .SetMarginBottom(10);
                        document.Add(hospitalInfoParagraph);

                        // Add title
                        document.Add(new Paragraph("Allergies Report")
                            .SetFontSize(16)
                            .SetBold()
                            .SetTextAlignment(TextAlignment.CENTER)
                            .SetMarginBottom(10));

                        // Create table for allergies
                        var table = new Table(UnitValue.CreatePercentArray(new float[] { 1, 3, 6 })) // Three columns
                            .SetWidth(UnitValue.CreatePercentValue(100));

                        // Add header
                        table.AddHeaderCell(new Cell().Add(new Paragraph("Allergy ID").SetFontSize(8)));
                        table.AddHeaderCell(new Cell().Add(new Paragraph("Name").SetFontSize(8)));
                        table.AddHeaderCell(new Cell().Add(new Paragraph("Description").SetFontSize(8)));

                        // Add data to the table
                        foreach (var allergy in allergies)
                        {
                            table.AddCell(new Cell().Add(new Paragraph(allergy.AllergyId.ToString()).SetFontSize(8)));
                            table.AddCell(new Cell().Add(new Paragraph(allergy.Name).SetFontSize(8)));
                            table.AddCell(new Cell().Add(new Paragraph(allergy.Description).SetFontSize(8)));
                        }

                        // Add the table to the document
                        document.Add(table);

                        // Summary Section (reduce font size and margins)
                        document.Add(new Paragraph("\nSummary:")
                            .SetBold()
                            .SetFontSize(10)
                            .SetMarginTop(10));

                        document.Add(new Paragraph($"Total Allergies: {allergies.Count}")
                            .SetFontSize(8)); // Reduced font size

                        document.Add(new Paragraph($"Generated on: {DateTime.Now.ToString("MM/dd/yyyy HH:mm")}")
                            .SetFontSize(8)); // Reduced font size

                        document.Close();
                    }

                    // Return the PDF file
                    var pdfBytes = memoryStream.ToArray();
                    return File(pdfBytes, "application/pdf", "Allergies.pdf");
                }
            }
            catch (Exception ex)
            {
                // Log the exception (use your logging framework)
                // Return an appropriate error view/message
                return BadRequest("An error occurred while generating the PDF.");
            }
        }








































        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkDeleteConditions(string selectedConditions)
        {
            // Log the method entry for debugging
            Console.WriteLine("BulkDeleteConditions called.");

            if (string.IsNullOrEmpty(selectedConditions))
            {
                TempData["ErrorMessage"] = "No conditions were selected for deletion.";
                return RedirectToAction(nameof(Conditions));
            }

            // Split the comma-separated condition IDs into an array
            var conditionIds = selectedConditions.Split(',').Select(int.Parse).ToArray();

            // Log selected condition IDs for debugging
            foreach (var id in conditionIds)
            {
                Console.WriteLine($"Selected Condition ID: {id}");
            }

            var conditionsToDelete = await _context.Conditions
                .Where(c => conditionIds.Contains(c.ConditionId) && c.DeletionStatus == ConditionStatus.Active)
                .ToListAsync();

            if (!conditionsToDelete.Any())
            {
                TempData["ErrorMessage"] = "No conditions found for deletion.";
                return RedirectToAction(nameof(Conditions));
            }

            // Perform the soft delete
            foreach (var condition in conditionsToDelete)
            {
                condition.DeletionStatus = ConditionStatus.Deleted; // Soft delete
                _context.Update(condition);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Selected conditions deleted successfully!";
            return RedirectToAction(nameof(Conditions));
        }

        [HttpGet]
        public async Task<IActionResult> GetConditionNames(string term)
        {
            var conditionNames = await _context.Conditions
                .Where(c => c.DeletionStatus == ConditionStatus.Active && c.Name.Contains(term))
                .Select(c => c.Name)
                .ToListAsync();

            return Json(conditionNames);
        }






        public async Task<IActionResult> Conditions(int pageNumber = 1, int pageSize = 4, string searchString = "", string sortOrder = "asc")
        {
            ViewData["ShowSidebar"] = true;
            ViewData["CurrentSearch"] = searchString;
            ViewData["CurrentSortOrder"] = sortOrder;

            // Get the queryable conditions based on the search string
            var conditionsQuery = _context.Conditions
                .Where(c => c.DeletionStatus == ConditionStatus.Active);

            if (!string.IsNullOrEmpty(searchString))
            {
                conditionsQuery = conditionsQuery.Where(c => c.Name.Contains(searchString));
            }

            // Apply sorting based on the sortOrder parameter
            if (sortOrder == "asc")
            {
                conditionsQuery = conditionsQuery.OrderBy(c => c.Name);
            }
            else
            {
                conditionsQuery = conditionsQuery.OrderByDescending(c => c.Name);
            }

            // Get the total count of filtered conditions
            var totalConditions = await conditionsQuery.CountAsync();

            // Calculate total pages for pagination
            var totalPages = (int)Math.Ceiling(totalConditions / (double)pageSize);

            // Fetch filtered, sorted, and paginated conditions
            var conditions = await conditionsQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewData["PageNumber"] = pageNumber;
            ViewData["TotalPages"] = totalPages;

            return View(conditions);
        }


        public IActionResult AddCondition()
        {
            ViewData["ShowSidebar"] = true;

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCondition([Bind("ConditionId,Name,Description")] Condition condition)
        {
            
                // Check for existing condition with the same name and active status
                bool exists = await _context.Conditions
                    .AnyAsync(c => c.Name.Equals(condition.Name, StringComparison.OrdinalIgnoreCase) && c.DeletionStatus == ConditionStatus.Active);

                if (exists)
                {
                    TempData["ErrorMessage"] = "A condition with this name already exists. Please use a different name.";
                    return View(condition);
                }

                condition.DeletionStatus = ConditionStatus.Active; // Set default status
                _context.Add(condition);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Condition added successfully!";
                return RedirectToAction(nameof(Conditions));
            

            TempData["ErrorMessage"] = "Failed to add condition. Please check the input and try again.";
            return View(condition);
        }


        public async Task<IActionResult> EditCondition(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewData["ShowSidebar"] = true;

            var condition = await _context.Conditions.FindAsync(id);
            if (condition == null || condition.DeletionStatus == ConditionStatus.Deleted)
            {
                return NotFound();
            }
            return View(condition);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCondition(int id, [Bind("ConditionId,Name,Description,DeletionStatus")] Condition condition)
        {
            if (id != condition.ConditionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(condition);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Condition updated successfully!";
                    return RedirectToAction(nameof(Conditions));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ConditionExists(condition.ConditionId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Error updating condition. Please try again.";
                        throw;
                    }
                }
            }

            TempData["ConditionErrorMessage"] = "Failed to update condition. Please check the input and try again.";
            return View(condition);
        }

        public async Task<IActionResult> DeleteCondition(int? id)
        {
            ViewData["ShowSidebar"] = true;
            if (id == null)
            {
                return NotFound();
            }

            var condition = await _context.Conditions
                .FirstOrDefaultAsync(c => c.ConditionId == id && c.DeletionStatus == ConditionStatus.Active);
            if (condition == null)
            {
                return NotFound();
            }

            return View(condition);
        }

        [HttpPost, ActionName("DeleteCondition")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConditionConfirmed(int id)
        {
            var condition = await _context.Conditions.FindAsync(id);
            ViewData["ShowSidebar"] = true;

            if (condition == null || condition.DeletionStatus == ConditionStatus.Deleted)
            {
                return NotFound();
            }

            // Soft delete condition
            condition.DeletionStatus = ConditionStatus.Deleted;
            _context.Update(condition);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Condition deleted successfully!";
            return RedirectToAction(nameof(Conditions));
        }

        private bool ConditionExists(int id)
        {
            return _context.Conditions.Any(e => e.ConditionId == id && e.DeletionStatus == ConditionStatus.Active);
        }



        [HttpGet("DeletedConditions")]
        public async Task<IActionResult> DeletedConditions(int pageNumber = 1, int pageSize = 4)
        {
            ViewData["ShowSidebar"] = true;

            var deletedConditions = await _context.Conditions
                .Where(c => c.DeletionStatus == ConditionStatus.Deleted)
                .OrderBy(c => c.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Get total count for pagination
            var totalDeletedConditions = await _context.Conditions.CountAsync(c => c.DeletionStatus == ConditionStatus.Deleted);
            var totalPages = (int)Math.Ceiling(totalDeletedConditions / (double)pageSize);

            ViewData["PageNumber"] = pageNumber;
            ViewData["TotalPages"] = totalPages;

            return View(deletedConditions);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreCondition(int id)
        {
            var condition = await _context.Conditions.FindAsync(id);

            if (condition == null || condition.DeletionStatus == ConditionStatus.Active)
            {
                return NotFound();
            }

            // Restore the deleted condition
            condition.DeletionStatus = ConditionStatus.Active;
            _context.Update(condition);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Condition restored successfully!";
            return RedirectToAction(nameof(DeletedConditions));
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkRestoreConditions(string selectedConditions)
        {
            if (string.IsNullOrWhiteSpace(selectedConditions))
            {
                TempData["ErrorMessage"] = "No conditions selected for restoration.";
                return RedirectToAction(nameof(DeletedConditions));
            }

            // Split the comma-separated condition IDs into an array
            var conditionIds = selectedConditions.Split(',').Select(int.Parse).ToArray();

            // Fetch deleted conditions matching the selected IDs
            var conditionsToRestore = await _context.Conditions
                .Where(c => conditionIds.Contains(c.ConditionId) && c.DeletionStatus == ConditionStatus.Deleted)
                .ToListAsync();

            if (!conditionsToRestore.Any())
            {
                TempData["ErrorMessage"] = "No conditions found for restoration.";
                return RedirectToAction(nameof(DeletedConditions));
            }

            // Restore the selected conditions
            foreach (var condition in conditionsToRestore)
            {
                condition.DeletionStatus = ConditionStatus.Active; // Restore
                _context.Update(condition);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Selected conditions restored successfully!";
            return RedirectToAction(nameof(DeletedConditions));
        }



        [HttpGet]
        public async Task<IActionResult> ExportConditionsToPdf()
        {
            try
            {
                // Get hospital information
                var hospitalInfo = await _context.HospitalInfos.FirstOrDefaultAsync();

                if (hospitalInfo == null)
                {
                    return BadRequest("Hospital information is missing.");
                }

                // Retrieve conditions from the database
                var conditions = await _context.Conditions
                    .Where(c => c.DeletionStatus == ConditionStatus.Active)
                    .ToListAsync();

                using (var memoryStream = new MemoryStream())
                {
                    // Create PDF document
                    var pdfWriter = new PdfWriter(memoryStream);
                    using (var pdfDocument = new PdfDocument(pdfWriter))
                    {
                        // Set document to have narrow margins
                        var document = new Document(pdfDocument, PageSize.A4);
                        document.SetMargins(20, 20, 20, 20); // Set smaller margins

                        // Add the hospital logo (reduce the size to 50 for a compact layout)
                        var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "LogoResized.png");
                        if (System.IO.File.Exists(logoPath))
                        {
                            var logoImageData = ImageDataFactory.Create(logoPath);
                            var logo = new iText.Layout.Element.Image(logoImageData)
                                .SetWidth(50) // Reduce width for better fit
                                .SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.LEFT)
                                .SetMarginBottom(10);
                            document.Add(logo);
                        }

                        // Add hospital info at the top left corner (reduce font size for compact layout)
                        var hospitalInfoParagraph = new Paragraph($"{hospitalInfo.HospitalName}\n{hospitalInfo.Address}\n{hospitalInfo.PhoneNumber}")
                            .SetFontSize(8) // Reduced font size
                            .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
                            .SetMarginBottom(10);
                        document.Add(hospitalInfoParagraph);

                        // Add title
                        document.Add(new Paragraph("Conditions Report")
                            .SetFontSize(16)
                            .SetBold()
                            .SetTextAlignment(TextAlignment.CENTER)
                            .SetMarginBottom(10));

                        // Create table for conditions
                        var table = new Table(UnitValue.CreatePercentArray(new float[] { 1, 3, 6 })) // Three columns
                            .SetWidth(UnitValue.CreatePercentValue(100));

                        // Add header
                        table.AddHeaderCell(new Cell().Add(new Paragraph("Condition ID").SetFontSize(8)));
                        table.AddHeaderCell(new Cell().Add(new Paragraph("Name").SetFontSize(8)));
                        table.AddHeaderCell(new Cell().Add(new Paragraph("Description").SetFontSize(8)));

                        // Populate table with conditions
                        foreach (var condition in conditions)
                        {
                            table.AddCell(new Cell().Add(new Paragraph(condition.ConditionId.ToString()).SetFontSize(8)));
                            table.AddCell(new Cell().Add(new Paragraph(condition.Name).SetFontSize(8)));
                            table.AddCell(new Cell().Add(new Paragraph(condition.Description).SetFontSize(8)));
                        }

                        // Add the table to the document
                        document.Add(table);

                        // Summary Section (reduce font size and margins)
                        document.Add(new Paragraph("\nSummary:")
                            .SetBold()
                            .SetFontSize(10)
                            .SetMarginTop(10));

                        document.Add(new Paragraph($"Total Conditions: {conditions.Count}")
                            .SetFontSize(8)); // Reduced font size

                        document.Add(new Paragraph($"Generated on: {DateTime.Now.ToString("MM/dd/yyyy HH:mm")}")
                            .SetFontSize(8)); // Reduced font size

                        document.Close();
                    }

                    // Return the PDF file
                    var pdfBytes = memoryStream.ToArray();
                    return File(pdfBytes, "application/pdf", "ConditionsReport.pdf");
                }
            }
            catch (Exception ex)
            {
                // Log the exception (use your logging framework)
                // Return an appropriate error view/message
                return BadRequest("An error occurred while generating the PDF.");
            }
        }










































        public IActionResult HospitalInfo()
        {
            ViewData["ShowSidebar"] = true;

            var hospitalInfoList = _context.HospitalInfos.ToList();
            return View(hospitalInfoList);
        }

        // GET: HospitalInfo/Create
        public IActionResult AddHospital()
        {
            ViewData["ShowSidebar"] = true;

            return View();
        }

        // POST: HospitalInfo/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddHospital([Bind("HospitalName,Address,PhoneNumber,Email,WebsiteUrl")] HospitalInfo hospitalInfo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hospitalInfo);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Hospital information added successfully!";
                return RedirectToAction(nameof(HospitalInfo));
            }

            TempData["ErrorMessage"] = "Failed to add hospital information. Please check the input and try again.";
            return View(hospitalInfo);
        }

        public IActionResult EditHospital()
        {
            ViewData["ShowSidebar"] = true;

            var hospitalInfo = _context.HospitalInfos.FirstOrDefault();
            if (hospitalInfo == null)
            {
                return NotFound();
            }
            return View(hospitalInfo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditHospital([Bind("HospitalInfoId,HospitalName,Address,PhoneNumber,Email,WebsiteUrl")] HospitalInfo hospitalInfo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hospitalInfo);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Hospital information updated successfully!";
                    return RedirectToAction(nameof(HospitalInfo));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HospitalInfoExists(hospitalInfo.HospitalInfoId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        TempData["HospitalErrorMessage"] = "Error updating hospital information. Please try again.";
                        throw;
                    }
                }
            }

            TempData["HospitalErrorMessage"] = "Failed to update hospital information. Please check the input and try again.";
            return View(hospitalInfo);
        }


































        public async Task<IActionResult> ListOfUsers(int pageNumber = 1, int pageSize = 4, string sortOrder = "")
        {
            ViewData["ShowSidebar"] = true;

            // Determine the sorting order
            IQueryable<ApplicationUser> usersQuery = _context.Register;

            switch (sortOrder)
            {
                case "name_desc":
                    usersQuery = usersQuery.OrderByDescending(u => u.FirstName);
                    break;
                case "email":
                    usersQuery = usersQuery.OrderBy(u => u.Email);
                    break;
                case "email_desc":
                    usersQuery = usersQuery.OrderByDescending(u => u.Email);
                    break;
                case "role":
                    usersQuery = usersQuery.OrderBy(u => u.Role);
                    break;
                case "role_desc":
                    usersQuery = usersQuery.OrderByDescending(u => u.Role);
                    break;
                default: // Name ascending 
                    usersQuery = usersQuery.OrderBy(u => u.FirstName);
                    break;
            }

            // Pagination
            var totalUsers = await usersQuery.CountAsync();
            var totalPages = (int)Math.Ceiling(totalUsers / (double)pageSize);

            var users = await usersQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserListViewModel
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    Role = u.Role.ToString(),
                    Status = u.Status.ToString()
                })
                .ToListAsync();

            ViewData["PageNumber"] = pageNumber;
            ViewData["TotalPages"] = totalPages;
            ViewData["CurrentSort"] = sortOrder;

            return View(users);
        }

        private bool HospitalInfoExists(int id)
        {
            return _context.HospitalInfos.Any(e => e.HospitalInfoId == id);
        }









    }
}














































