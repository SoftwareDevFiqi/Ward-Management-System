using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimelessTechnicians.UI.Data;
using TimelessTechnicians.UI.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TimelessTechnicians.UI.Controllers
{
    [Authorize(Roles = "SCRIPTMANAGER")]
    public class ScriptManagerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ScriptManagerController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> ScriptManagerDashboard()
        {
            var currentUserName = User.Identity.Name;
            ViewData["ShowSidebar"] = true;

            // Fetch statistics
            var totalPrescriptions = await _context.Prescriptions.CountAsync();
            var totalDoctors = await _context.Users.CountAsync(u => u.Role == ApplicationUser.UserRole.DOCTOR);

            // Fetch prescription status counts
            var totalForwarded = await _context.Prescriptions.CountAsync(p => p.Status == InstructionStatus.ForwardedToPharmacy);
            var totalDelivered = await _context.Prescriptions.CountAsync(p => p.Status == InstructionStatus.Delivered);
            var totalReceived = await _context.Prescriptions.CountAsync(p => p.Status == InstructionStatus.Received);

            // Store statistics in ViewData
            ViewData["TotalPrescriptions"] = totalPrescriptions;
            ViewData["TotalDoctors"] = totalDoctors;
            ViewData["TotalForwarded"] = totalForwarded;
            ViewData["TotalDelivered"] = totalDelivered;
            ViewData["TotalReceived"] = totalReceived;

            return View();
        }


        // List all prescriptions for the ScriptManager
        public async Task<IActionResult> ListPrescriptions()
        {
            ViewData["ShowSidebar"] = true;

            // Retrieve all prescriptions with related Patient and Doctor data
            var prescriptions = await _context.Prescriptions
                .Include(p => p.Patient)
                .Include(p => p.Doctor) // Include Doctor details
                .ToListAsync();

            return View(prescriptions); // Pass the list of prescriptions to the view
        }




        public async Task<IActionResult> ForwardToPharmacy(int id)
        {
            ViewData["ShowSidebar"] = true;
            var prescription = await _context.Prescriptions.FindAsync(id);
            if (prescription == null)
            {
                return NotFound();
            }

            // Implement logic to forward prescription to the pharmacy
            prescription.Status = InstructionStatus.ForwardedToPharmacy;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ListPrescriptions));
        }

        public async Task<IActionResult> MarkAsDelivered(int id)
        {
            ViewData["ShowSidebar"] = true;

            var prescription = await _context.Prescriptions.FindAsync(id);
            if (prescription == null)
            {
                return NotFound();
            }

            // Implement logic to mark prescription as delivered
            prescription.Status = InstructionStatus.Delivered;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ListPrescriptions));
        }



        // GET: Receive Medication
        [HttpGet]
        public async Task<IActionResult> ReceiveMedication(int id)
        {
            ViewData["ShowSidebar"] = true;

            var prescription = await _context.Prescriptions
                .Include(p => p.Patient)
                .Include(p => p.Doctor)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (prescription == null)
            {
                return NotFound();
            }

            // Display the prescription details for verification
            return View(prescription);
        }

        // POST: Receive Medication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReceiveMedicationConfirmed(int id)
        {
            ViewData["ShowSidebar"] = true;

            var prescription = await _context.Prescriptions.FindAsync(id);
            if (prescription == null)
            {
                return NotFound();
            }

            // Mark the prescription as received
            prescription.Status = InstructionStatus.Received; // Update the status to Received
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ListPrescriptions));
        }

    }
}
