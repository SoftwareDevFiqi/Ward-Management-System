using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TimelessTechnicians.UI.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TimelessTechnicians.UI.Models;
using System.Security.Claims;

namespace TimelessTechnicians.UI.Controllers
{
    [Authorize(Roles = "PATIENT")]
    public class PatientController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PatientController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> PatientDashboard()
        {
            var currentUserName = User.Identity.Name;
            ViewData["ShowSidebar"] = true;

            // Fetch the hospital information from the database
            var hospitalInfo = await _context.HospitalInfos.FirstOrDefaultAsync();

            // Log if no hospital information was found
            if (hospitalInfo == null)
            {
                // Optionally log or set a TempData message
                Console.WriteLine("No hospital information found.");
            }

            // Pass the hospital information to the view
            return View(hospitalInfo);
        }


      



    }
}
