using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TimelessTechnicians.UI.Data;
using TimelessTechnicians.UI.Models;
using TimelessTechnicians.UI.ViewModel;

namespace TimelessTechnicians.UI.Controllers
{
    [Authorize(Roles = "NURSINGSISTER")]
    public class NursingSisterController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NursingSisterController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> NursingSisterDashboard()
        {
            ViewData["ShowSidebar"] = true;

            var currentUserName = User.Identity.Name;

            // Total administered medications by the logged-in user
            var totalAdministeredMedications = await _context.ScheduledMedications
                .Where(sm => sm.AdministeredBy == currentUserName)
                .CountAsync();

            // Medications administered by schedule
            var medicationBySchedule = await _context.ScheduledMedications
                .Where(sm => sm.AdministeredBy == currentUserName)
                .GroupBy(sm => sm.Medication.Schedule)
                .Select(g => new MedicationScheduleGroup
                {
                    Schedule = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            // Medications administered per month
            var medicationPerMonth = await _context.ScheduledMedications
                .Where(sm => sm.AdministeredBy == currentUserName)
                .GroupBy(sm => sm.AdministeredDate.Month)
                .Select(g => new MedicationMonthGroup
                {
                    Month = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            // Medications status count (completed, pending, etc.)
            var medicationStatusCount = await _context.ScheduledMedications
                .Where(sm => sm.AdministeredBy == currentUserName)
                .GroupBy(sm => sm.ScheduledMedicationStatus)
                .Select(g => new MedicationStatusGroup
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            // Average dosage of administered medications
            // Fetch all dosage values where the user is the current user
            var dosageValues = await _context.ScheduledMedications
                .Where(sm => sm.AdministeredBy == currentUserName)
                .Select(sm => sm.Dosage)
                .ToListAsync();

            // Filter out invalid dosage values and parse valid ones
            var validDosages = dosageValues
                .Where(d => decimal.TryParse(d, out _)) // Keep only valid numeric strings
                .Select(d => decimal.Parse(d)); // Convert strings to decimal

            // Calculate the average
            var averageDosage = validDosages.Any() ? validDosages.Average() : 0;


            // Count of deleted medications
            var deletedMedicationsCount = await _context.ScheduledMedications
                .Where(sm => sm.AdministeredBy == currentUserName && sm.ScheduledMedicationStatus == ScheduledMedicationStatus.Deleted)
                .CountAsync();

            // Create a dashboard stats view model
            var dashboardStats = new NursingSisterDashboardViewModel
            {
                TotalAdministeredMedications = totalAdministeredMedications,
                MedicationBySchedule = medicationBySchedule,
                MedicationPerMonth = medicationPerMonth,
                MedicationStatusCount = medicationStatusCount,
                AverageDosage = (double)averageDosage, 
                DeletedMedicationsCount = deletedMedicationsCount
            };


            return View(dashboardStats);
        }


        // GET: List Scheduled Medications
        public async Task<IActionResult> ListScheduledMedications()
        {
            ViewData["ShowSidebar"] = true;

            var currentUserName = User.Identity.Name; // Get the logged-in nursing sister's identity

            var scheduledMedications = await _context.ScheduledMedications
                .Where(sm => sm.ScheduledMedicationStatus != ScheduledMedicationStatus.Deleted
                             && sm.AdministeredBy == currentUserName) // Filter by nursing sister
                .Include(sm => sm.Medication)  // Include medication details
                .Select(sm => new ScheduledMedicationListViewModel
                {
                    ScheduledMedicationId = sm.Id,
                    MedicationId = sm.MedicationId,
                    MedicationName = sm.Medication.Name,
                    Dosage = sm.Dosage,
                    AdministeredDate = sm.AdministeredDate,
                    AdministeredBy = sm.AdministeredBy,
                    ScheduledMedicationStatus = sm.ScheduledMedicationStatus.ToString()
                })
                .ToListAsync();

            return View(scheduledMedications);
        }

        public async Task<IActionResult> AdministerScheduledMedication()
        {
            ViewData["ShowSidebar"] = true;

            // Fetch active medications that Nursing Sisters are allowed to administer
            var medications = await _context.Medications
                .Where(m => m.DeletionStatus == MedicationStatus.Active && m.Schedule >= MedicationSchedule.Schedule5)
                .Select(m => new
                {
                    m.MedicationId,
                    m.Name
                })
                .ToListAsync();

            var model = new ScheduledMedicationViewModel
            {
                AdministeredDate = DateTime.Now
            };

            // Populate the medication dropdown list
            ViewBag.MedicationList = new SelectList(medications, "MedicationId", "Name");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdministerScheduledMedication(ScheduledMedicationViewModel model)
        {
            var currentUserName = User.Identity.Name; // Ensure current user context

            if (!ModelState.IsValid)
            {
                await PopulateMedicationList(model.MedicationId); // Helper method to populate medication list
                TempData["AdministerMedicationError"] = "Please check the details and try again.";
                return View(model);
            }

            var medication = await _context.Medications
                .FirstOrDefaultAsync(m => m.MedicationId == model.MedicationId);

            if (medication == null)
            {
                ModelState.AddModelError("", "Invalid medication selected.");
                TempData["AdministerMedicationError"] = "Invalid medication selected.";
                return View(model);
            }

            // Ensure only authorized roles can administer certain medications
            if (medication.Schedule >= MedicationSchedule.Schedule5 && !User.IsInRole("NURSINGSISTER"))
            {
                ModelState.AddModelError("", "Only a Nursing Sister can administer Schedule 5 or higher medications.");
                TempData["AdministerMedicationError"] = "Only a Nursing Sister can administer Schedule 5 or higher medications.";
                return View(model);
            }

            // Create the scheduled medication record with user context
            var scheduledMedication = new ScheduledMedication
            {
                MedicationId = model.MedicationId,
                Dosage = model.Dosage,
                AdministeredDate = model.AdministeredDate,
                AdministeredBy = currentUserName, // Set the current user as the administering nurse
                ScheduledMedicationStatus = ScheduledMedicationStatus.Completed
            };

            _context.ScheduledMedications.Add(scheduledMedication);
            await _context.SaveChangesAsync();

            TempData["AdministerMedicationSuccess"] = "Scheduled medication administered successfully.";
            TempData["MessageType"] = "success";
            return RedirectToAction("ListScheduledMedications");
        }

        public async Task<IActionResult> EditScheduledMedication(int id)
        {
            ViewData["ShowSidebar"] = true;
            var currentUserName = User.Identity.Name;

            var scheduledMedication = await _context.ScheduledMedications
                .Include(sm => sm.Medication)
                .FirstOrDefaultAsync(sm => sm.Id == id && sm.AdministeredBy == currentUserName); // Filter by current user

            if (scheduledMedication == null)
            {
                return NotFound();
            }

            var model = new ScheduledMedicationViewModel
            {
                ScheduledMedicationId = scheduledMedication.Id,
                MedicationId = scheduledMedication.MedicationId,
                Dosage = scheduledMedication.Dosage,
                AdministeredDate = scheduledMedication.AdministeredDate,
                AdministeredBy = scheduledMedication.AdministeredBy,
                ScheduledMedicationStatus = scheduledMedication.ScheduledMedicationStatus
            };

            await PopulateMedicationList(model.MedicationId); // Helper method to populate medication list
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditScheduledMedication(int id, ScheduledMedicationViewModel model)
        {
            var currentUserName = User.Identity.Name;

            if (!ModelState.IsValid)
            {
                await PopulateMedicationList(model.MedicationId); // Helper method to populate medication list
                return View(model);
            }

            var scheduledMedication = await _context.ScheduledMedications
                .FirstOrDefaultAsync(sm => sm.Id == id && sm.AdministeredBy == currentUserName); // Ensure only current user can edit

            if (scheduledMedication == null)
            {
                return NotFound();
            }

            // Update the scheduled medication fields
            scheduledMedication.MedicationId = model.MedicationId;
            scheduledMedication.Dosage = model.Dosage;
            scheduledMedication.AdministeredDate = model.AdministeredDate;
            scheduledMedication.ScheduledMedicationStatus = model.ScheduledMedicationStatus;

            try
            {
                _context.Update(scheduledMedication);
                await _context.SaveChangesAsync();

                TempData["AdministerMedicationSuccess"] = "Scheduled medication updated successfully.";
                TempData["MessageType"] = "success";
                return RedirectToAction("ListScheduledMedications");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ScheduledMedicationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        [HttpPost, ActionName("DeleteScheduledMedication")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteScheduledMedication(int id)
        {
            var currentUserName = User.Identity.Name;

            var scheduledMedication = await _context.ScheduledMedications
                .FirstOrDefaultAsync(sm => sm.Id == id && sm.AdministeredBy == currentUserName); // Ensure only current user can delete

            if (scheduledMedication == null)
            {
                return NotFound();
            }

            // Mark the medication as deleted
            scheduledMedication.ScheduledMedicationStatus = ScheduledMedicationStatus.Deleted;

            _context.Update(scheduledMedication);
            await _context.SaveChangesAsync();

            TempData["AdministerMedicationSuccess"] = "Scheduled medication marked as deleted.";
            TempData["MessageType"] = "success";
            return RedirectToAction("ListScheduledMedications");
        }

        // Helper method to populate medication list in dropdown
        private async Task PopulateMedicationList(int? selectedMedicationId = null)
        {
            var medications = await _context.Medications
                .Where(m => m.DeletionStatus == MedicationStatus.Active && m.Schedule >= MedicationSchedule.Schedule5)
                .Select(m => new
                {
                    m.MedicationId,
                    m.Name
                })
                .ToListAsync();

            ViewBag.MedicationList = new SelectList(medications, "MedicationId", "Name", selectedMedicationId);
        }

        private bool ScheduledMedicationExists(int id)
        {
            return _context.ScheduledMedications.Any(sm => sm.Id == id);
        }
    }
}
