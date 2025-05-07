using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TimelessTechnicians.UI.Data;
using TimelessTechnicians.UI.Models;
using TimelessTechnicians.UI.ViewModel;
using static TimelessTechnicians.UI.Models.ApplicationUser;

namespace TimelessTechnicians.UI.Controllers
{
    [Authorize(Roles = "DOCTOR,NURSE,NURSINGSISTER")]
    public class NurseController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NurseController(ApplicationDbContext context)
        {
            _context = context;
        }




        public async Task<IActionResult> NurseDashboard()
        {
            ViewData["ShowSidebar"] = true;

            // Fetch statistics
            var totalPatients = await _context.Users.CountAsync(u => u.Role == UserRole.PATIENT);
            var totalAdmittedPatients = await _context.AdmitPatients.CountAsync(ap => ap.AdmitPatientStatus == AdmitPatientStatus.Admitted);
            var totalDischargedPatients = await _context.AdmitPatients.CountAsync(ap => ap.AdmitPatientStatus == AdmitPatientStatus.Discharged);
            var totalMedications = await _context.Medications.CountAsync();

            // Store statistics in ViewData
            ViewData["TotalPatients"] = totalPatients;
            ViewData["TotalAdmittedPatients"] = totalAdmittedPatients;
            ViewData["TotalDischargedPatients"] = totalDischargedPatients;
           
            ViewData["TotalMedications"] = totalMedications;

            return View();
        }














        // GET: Nurse/RecordVitals
        public async Task<IActionResult> RecordVitals()
        {
            ViewData["ShowSidebar"] = true;
            var nurseId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Fetch the logged-in nurse's ID

            var admittedPatients = await _context.AdmitPatients
                .Include(ap => ap.Patient)
                .Where(ap => ap.AdmitPatientStatus == AdmitPatientStatus.Admitted) // Ensure patient is admitted
                .Select(ap => new
                {
                    ap.Patient.Id,
                    FullName = $"{ap.Patient.FirstName} {ap.Patient.LastName}"
                })
                .ToListAsync();

            ViewBag.PatientList = new SelectList(admittedPatients, "Id", "FullName");

            // Populate the status dropdown
            ViewBag.PatientVitalStatusList = new SelectList(Enum.GetValues(typeof(PatientVitalStatus)));

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> RecordVitals(PatientVitalViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if the selected patient is admitted
                var isPatientAdmitted = await _context.AdmitPatients
                    .AnyAsync(ap => ap.PatientId == model.PatientId && ap.AdmitPatientStatus == AdmitPatientStatus.Admitted);

                if (!isPatientAdmitted)
                {
                    ModelState.AddModelError("PatientId", "The selected patient is not currently admitted.");
                    // Re-populate patient list and status dropdown
                    var admittedPatients = await _context.AdmitPatients
                        .Include(ap => ap.Patient)
                        .Where(ap => ap.AdmitPatientStatus == AdmitPatientStatus.Admitted)
                        .Select(ap => new
                        {
                            ap.Patient.Id,
                            FullName = $"{ap.Patient.FirstName} {ap.Patient.LastName}"
                        })
                        .ToListAsync();

                    ViewBag.PatientList = new SelectList(admittedPatients, "Id", "FullName");
                    ViewBag.PatientVitalStatusList = new SelectList(Enum.GetValues(typeof(PatientVitalStatus)));

                    TempData["ErrorMessage"] = "Error recording vitals. Please check the details.";
                    return View(model);
                }

                var patientVital = new PatientVital
                {
                    PatientId = model.PatientId,
                    BloodPressure = model.BloodPressure,
                    Temperature = model.Temperature,
                    SugarLevel = model.SugarLevel,
                    RecordedDate = model.RecordedDate,
                    PatientVitalStatus = model.PatientVitalStatus,
                    AdministeredBy = User.Identity.Name 
                };

                _context.PatientVitals.Add(patientVital);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Vitals recorded successfully.";
                return RedirectToAction("ListVitals");
            }

            // If ModelState is not valid, repopulate ViewBag.PatientList and ViewBag.PatientVitalStatusList
            var admittedPatientsFallback = await _context.AdmitPatients
                .Include(ap => ap.Patient)
                .Where(ap => ap.AdmitPatientStatus == AdmitPatientStatus.Admitted)
                .Select(ap => new
                {
                    ap.Patient.Id,
                    FullName = $"{ap.Patient.FirstName} {ap.Patient.LastName}"
                })
                .ToListAsync();

            ViewBag.PatientList = new SelectList(admittedPatientsFallback, "Id", "FullName");
            ViewBag.PatientVitalStatusList = new SelectList(Enum.GetValues(typeof(PatientVitalStatus)));

            TempData["ErrorMessage"] = "Error recording vitals. Please check the details.";
            return View(model);
        }


        // GET: Nurse/ListVitals
        public async Task<IActionResult> ListVitals()
        {
            ViewData["ShowSidebar"] = true;
            var nurseId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Fetch the logged-in nurse's ID

            // Fetch the patient vitals that are administered by the current nurse and are active
            var patientVitals = await _context.PatientVitals
                .Include(pv => pv.Patient)
                .Where(pv => pv.AdministeredBy == User.Identity.Name && pv.PatientVitalStatus == PatientVitalStatus.Active) // Filter by AdministeredBy and Active status
                .Select(pv => new PatientVitalListViewModel
                {
                    Id = pv.Id,
                    PatientFullName = $"{pv.Patient.FirstName} {pv.Patient.LastName}",
                    BloodPressure = pv.BloodPressure,
                    Temperature = pv.Temperature,
                    SugarLevel = pv.SugarLevel,
                    RecordedDate = pv.RecordedDate
                })
                .ToListAsync();

            return View(patientVitals);
        }


        // GET: Nurse/EditVitals/5
        public async Task<IActionResult> EditVitals(int id)
        {
            ViewData["ShowSidebar"] = true;
            var nurseId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Fetch the logged-in nurse's ID
            var patientVital = await _context.PatientVitals
                .Include(pv => pv.Patient)
                .FirstOrDefaultAsync(pv => pv.Id == id && pv.AdministeredBy == User.Identity.Name); // Ensure it's the nurse's own record

            if (patientVital == null)
            {
                TempData["ErrorMessage"] = "You are not authorized to edit this record.";
                return Forbid();
            }

            var model = new PatientVitalViewModel
            {
                Id = patientVital.Id,
                PatientId = patientVital.PatientId,
                BloodPressure = patientVital.BloodPressure,
                Temperature = patientVital.Temperature,
                SugarLevel = patientVital.SugarLevel,
                RecordedDate = patientVital.RecordedDate,
                PatientVitalStatus = patientVital.PatientVitalStatus // Include status
            };

            // Populate the patient dropdown list again if needed
            var patients = await _context.Users
                .Where(u => u.Role == ApplicationUser.UserRole.PATIENT)
                .Select(u => new
                {
                    u.Id,
                    FullName = $"{u.FirstName} {u.LastName}"
                })
                .ToListAsync();

            ViewBag.PatientList = new SelectList(patients, "Id", "FullName", model.PatientId);

            return View(model);
        }

        // POST: Nurse/EditVitals/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditVitals(int id, PatientVitalViewModel model)
        {
            if (ModelState.IsValid)
            {
                var patientVital = await _context.PatientVitals
                    .FirstOrDefaultAsync(pv => pv.Id == id && pv.AdministeredBy == User.Identity.Name); // Ensure it's the nurse's own record

                if (patientVital == null)
                {
                    TempData["ErrorMessage"] = "You are not authorized to edit this record.";
                    return Forbid();
                }

                patientVital.PatientId = model.PatientId;
                patientVital.BloodPressure = model.BloodPressure;
                patientVital.Temperature = model.Temperature;
                patientVital.SugarLevel = model.SugarLevel;
                patientVital.RecordedDate = model.RecordedDate;
                patientVital.PatientVitalStatus = model.PatientVitalStatus; // Set the status

                try
                {
                    _context.Update(patientVital);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Vitals updated successfully.";
                    return RedirectToAction("ListVitals");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientVitalExists(id))
                    {
                        TempData["ErrorMessage"] = "Concurrency error occurred.";
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            // Re-fetch the patients for the dropdown in case of validation failure
            var patients = await _context.Users
                .Where(u => u.Role == ApplicationUser.UserRole.PATIENT)
                .Select(u => new
                {
                    u.Id,
                    FullName = $"{u.FirstName} {u.LastName}"
                })
                .ToListAsync();
            ViewData["ShowSidebar"] = true;
            ViewBag.PatientList = new SelectList(patients, "Id", "FullName", model.PatientId);

            TempData["ErrorMessage"] = "Error updating vitals. Please check the details.";
            return View(model);
        }

        // DELETE: Nurse/DeleteVitals/5
        [HttpGet]
        public async Task<IActionResult> DeleteVitals(int id)
        {
            ViewData["ShowSidebar"] = true;
            var patientVital = await _context.PatientVitals
                .Include(pv => pv.Patient) // Assuming Patient navigation property exists
                .FirstOrDefaultAsync(pv => pv.Id == id && pv.AdministeredBy == User.Identity.Name); // Ensure it's the nurse's own record

            if (patientVital == null)
            {
                return NotFound();
            }

            var model = new PatientVitalViewModel
            {
                Id = patientVital.Id,
                PatientId = patientVital.PatientId,
                BloodPressure = patientVital.BloodPressure,
                Temperature = patientVital.Temperature,
                SugarLevel = patientVital.SugarLevel,
                RecordedDate = patientVital.RecordedDate,
                PatientVitalStatus = patientVital.PatientVitalStatus
            };

            return View(model); // Return the ViewModel to the view
        }

        // POST: Nurse/DeleteVitals/5
        [HttpPost, ActionName("DeleteVitals")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var patientVital = await _context.PatientVitals
                .FirstOrDefaultAsync(pv => pv.Id == id && pv.AdministeredBy == User.Identity.Name); // Ensure it's the nurse's own record

            if (patientVital == null)
            {
                TempData["ErrorMessage"] = "You are not authorized to delete this record.";
                return Forbid();
            }

            // Soft delete by setting the status to Deleted
            patientVital.PatientVitalStatus = PatientVitalStatus.Deleted;

            _context.Update(patientVital);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Vitals record deleted successfully.";
            return RedirectToAction("ListVitals");
        }






























































        // GET: Nurse/ListTreatments
        public async Task<IActionResult> ListTreatments()
        {
            ViewData["ShowSidebar"] = true;

            // Retrieve only active treatments for the current user
            var treatments = await _context.PatientTreatments
                .Where(t => t.TreatmentStatus == TreatmentStatus.Active && t.PerformedBy == User.Identity.Name)
                .Include(t => t.Patient) // Include patient navigation property
                .Select(t => new PatientTreatmentListViewModel
                {
                    Id = t.Id,
                    PatientFullName = $"{t.Patient.FirstName} {t.Patient.LastName}",
                    TreatmentDescription = t.TreatmentDescription,
                    DatePerformed = t.DatePerformed,
                    TreatmentStatus = t.TreatmentStatus // Include status in the view model
                })
                .ToListAsync();

            return View(treatments);
        }


        // GET: Nurse/RecordTreatment
        // GET: Nurse/RecordTreatment
        public async Task<IActionResult> RecordTreatment()
        {
            ViewData["ShowSidebar"] = true;

            // Get admitted patients
            var admittedPatients = await (from admit in _context.AdmitPatients
                                          join user in _context.Users on admit.PatientId equals user.Id
                                          where admit.AdmitPatientStatus == AdmitPatientStatus.Admitted
                                          select new
                                          {
                                              user.Id,
                                              FullName = $"{user.FirstName} {user.LastName}"
                                          }).ToListAsync();

            var model = new PatientTreatmentViewModel
            {
                DatePerformed = DateTime.Now, // Default to current date
                PatientList = new SelectList(admittedPatients, "Id", "FullName") // Only admitted patients
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecordTreatment(PatientTreatmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var treatment = new PatientTreatment
                {
                    PatientId = model.PatientId,
                    TreatmentDescription = model.TreatmentDescription,
                    DatePerformed = model.DatePerformed,
                    PerformedBy = User.Identity.Name // Set the PerformedBy field to the current user
                };

                _context.PatientTreatments.Add(treatment);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Treatment recorded successfully.";
                return RedirectToAction("ListTreatments");
            }

            // If ModelState is not valid, repopulate the dropdown with admitted patients
            var admittedPatients = await (from admit in _context.AdmitPatients
                                          join user in _context.Users on admit.PatientId equals user.Id
                                          where admit.AdmitPatientStatus == AdmitPatientStatus.Admitted
                                          select new
                                          {
                                              user.Id,
                                              FullName = $"{user.FirstName} {user.LastName}"
                                          }).ToListAsync();

            model.PatientList = new SelectList(admittedPatients, "Id", "FullName");
            TempData["ErrorMessage"] = "Treatment recording failed.";
            return View(model);
        }

        // GET: Nurse/EditTreatment/5
        // GET: Nurse/EditTreatment/5
        public async Task<IActionResult> EditTreatment(int id)
        {
            ViewData["ShowSidebar"] = true;

            var treatment = await _context.PatientTreatments
                .Include(t => t.Patient) // Include patient data
                .FirstOrDefaultAsync(t => t.Id == id && t.PerformedBy == User.Identity.Name); // Ensure nurse owns the treatment

            if (treatment == null)
            {
                return NotFound();
            }

            // Get admitted patients
            var admittedPatients = await (from admit in _context.AdmitPatients
                                          join user in _context.Users on admit.PatientId equals user.Id
                                          where admit.AdmitPatientStatus == AdmitPatientStatus.Admitted
                                          select new
                                          {
                                              user.Id,
                                              FullName = $"{user.FirstName} {user.LastName}"
                                          }).ToListAsync();

            var model = new PatientTreatmentViewModel
            {
                PatientId = treatment.PatientId,
                TreatmentDescription = treatment.TreatmentDescription,
                DatePerformed = treatment.DatePerformed,
                PerformedBy = treatment.PerformedBy,
                PatientList = new SelectList(admittedPatients, "Id", "FullName", treatment.PatientId),
                TreatmentStatus = treatment.TreatmentStatus
            };

            return View(model);
        }


        // POST: Nurse/EditTreatment/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTreatment(int id, PatientTreatmentViewModel model)
        {
            var treatment = await _context.PatientTreatments
                .FirstOrDefaultAsync(t => t.Id == id && t.PerformedBy == User.Identity.Name); // Ensure it's the nurse's own record

            if (treatment == null)
            {
                return NotFound();
            }

            // Always set TreatmentStatus to Active
            treatment.TreatmentStatus = TreatmentStatus.Active; // Ensure status is set to Active on edit

            // Update other properties
            treatment.PatientId = model.PatientId;
            treatment.TreatmentDescription = model.TreatmentDescription;
            treatment.DatePerformed = model.DatePerformed;

            try
            {
                _context.Update(treatment);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Treatment updated successfully.";
                return RedirectToAction("ListTreatments");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PatientTreatmentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            // Repopulate the dropdown if ModelState is not valid
            var patients = await _context.Users
                .Where(u => u.Role == ApplicationUser.UserRole.PATIENT)
                .Select(u => new
                {
                    u.Id,
                    FullName = $"{u.FirstName} {u.LastName}"
                })
                .ToListAsync();

            model.PatientList = new SelectList(patients, "Id", "FullName", model.PatientId);
            TempData["ErrorMessage"] = "Treatment recorded unsuccessfully.";
            return View(model);
        }


        // GET: Nurse/DeleteTreatment/5
        public async Task<IActionResult> DeleteTreatment(int id)
        {
            ViewData["ShowSidebar"] = true;
            var treatment = await _context.PatientTreatments
                .Include(t => t.Patient) // Include the patient data if needed
                .FirstOrDefaultAsync(t => t.Id == id && t.PerformedBy == User.Identity.Name); // Ensure it's the nurse's own record

            if (treatment == null)
            {
                return NotFound();
            }

            var model = new PatientTreatmentListViewModel
            {
                Id = treatment.Id,
                PatientFullName = $"{treatment.Patient.FirstName} {treatment.Patient.LastName}",
                TreatmentDescription = treatment.TreatmentDescription,
                DatePerformed = treatment.DatePerformed,
                TreatmentStatus = treatment.TreatmentStatus
            };

            return View(model);
        }

        // POST: Nurse/DeleteTreatment/5
        [HttpPost, ActionName("DeleteTreatment")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmTreatment(int id)
        {
            var treatment = await _context.PatientTreatments
                .FirstOrDefaultAsync(t => t.Id == id && t.PerformedBy == User.Identity.Name); // Ensure it's the nurse's own record

            if (treatment == null)
            {
                return NotFound();
            }

            // Set the status to Deleted instead of removing the record
            treatment.TreatmentStatus = TreatmentStatus.Deleted;
            _context.Update(treatment);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Treatment marked as deleted successfully.";
            TempData["MessageType"] = "success";

            return RedirectToAction("ListTreatments"); // Redirect to the treatments list
        }











































        // GET: Nurse/ListNonScheduledMedications
        public async Task<IActionResult> ListNonScheduledMedications()
        {
            ViewData["ShowSidebar"] = true;
            var nonScheduledMedications = await _context.NonScheduledMedications
                .Include(nsm => nsm.Medication)
                .Where(nsm => nsm.Status == NonScheduledMedicationStatus.Active && nsm.AdministeredBy == User.Identity.Name) // Filter by the current user
                .Select(nsm => new NonScheduledMedicationViewModel
                {
                    Id = nsm.Id,
                    MedicationName = nsm.Medication.Name,
                    Dosage = nsm.Dosage,
                    AdministeredDate = nsm.AdministeredDate,
                    AdministeredBy = nsm.AdministeredBy
                })
                .ToListAsync();

            return View(nonScheduledMedications);
        }

        // GET: Record Non-Scheduled Medication
        public async Task<IActionResult> RecordNonScheduledMedication()
        {
            ViewData["ShowSidebar"] = true;

            // Fetch list of active medications with Schedule up to Schedule 4
            var medications = await _context.Medications
                .Where(m => m.DeletionStatus == MedicationStatus.Active &&
                            m.Schedule <= MedicationSchedule.Schedule4)
                .Select(m => new
                {
                    m.MedicationId,
                    m.Name
                })
                .ToListAsync();

            var model = new NonScheduledMedicationViewModel
            {
                AdministeredDate = DateTime.Now
            };

            // Populate the medication dropdown list
            ViewBag.MedicationList = new SelectList(medications, "MedicationId", "Name");

            return View(model);
        }

        // POST: Record Non-Scheduled Medication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecordNonScheduledMedication(NonScheduledMedicationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["ShowSidebar"] = true;

                var medications = await _context.Medications
                    .Where(m => m.DeletionStatus == MedicationStatus.Active &&
                                m.Schedule <= MedicationSchedule.Schedule4)
                    .Select(m => new
                    {
                        m.MedicationId,
                        m.Name
                    })
                    .ToListAsync();
                TempData["ErrorMessage"] = "Record Non-Scheduled Medication recorded unsuccessfully.";

                ViewBag.MedicationList = new SelectList(medications, "MedicationId", "Name", model.MedicationId);

                return View(model);
            }

            // Save the non-scheduled medication record
            var medication = new NonScheduledMedication
            {
                MedicationId = model.MedicationId,
                Dosage = model.Dosage,
                AdministeredDate = model.AdministeredDate,
                AdministeredBy = User.Identity.Name, // Set the AdministeredBy field to the current user
                Status = NonScheduledMedicationStatus.Active
            };

            _context.NonScheduledMedications.Add(medication);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Non-scheduled medication recorded successfully.";
            return RedirectToAction("ListNonScheduledMedications");
        }

        // GET: Edit Non-Scheduled Medication
        public async Task<IActionResult> EditNonScheduledMedication(int id)
        {
            ViewData["ShowSidebar"] = true;
            var nonScheduledMedication = await _context.NonScheduledMedications
                .Include(nsm => nsm.Medication)
                .FirstOrDefaultAsync(nsm => nsm.Id == id && nsm.AdministeredBy == User.Identity.Name); // Ensure it's the nurse's own record

            if (nonScheduledMedication == null)
            {
                return NotFound();
            }

            var model = new NonScheduledMedicationViewModel
            {
                Id = nonScheduledMedication.Id,
                MedicationId = nonScheduledMedication.MedicationId,
                Dosage = nonScheduledMedication.Dosage,
                AdministeredDate = nonScheduledMedication.AdministeredDate,
                Status = nonScheduledMedication.Status
            };

            // Populate the medication dropdown list
            var medications = await _context.Medications
                .Where(m => m.DeletionStatus == MedicationStatus.Active)
                .Select(m => new
                {
                    m.MedicationId,
                    m.Name
                })
                .ToListAsync();

            ViewBag.MedicationList = new SelectList(medications, "MedicationId", "Name", model.MedicationId);

            return View(model);
        }

        // POST: Edit Non-Scheduled Medication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditNonScheduledMedication(int id, NonScheduledMedicationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var nonScheduledMedication = await _context.NonScheduledMedications
                    .FirstOrDefaultAsync(nsm => nsm.Id == id && nsm.AdministeredBy == User.Identity.Name); // Ensure it's the nurse's own record

                if (nonScheduledMedication == null)
                {
                    return NotFound();
                }

                nonScheduledMedication.MedicationId = model.MedicationId;
                nonScheduledMedication.Dosage = model.Dosage;
                nonScheduledMedication.AdministeredDate = model.AdministeredDate;
                nonScheduledMedication.Status = NonScheduledMedicationStatus.Active;

                try
                {
                    _context.Update(nonScheduledMedication);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Non-scheduled medication updated successfully.";
                    TempData["MessageType"] = "success";

                    return RedirectToAction("ListNonScheduledMedications");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NonScheduledMedicationExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            // Re-fetch the medications for the dropdown list in case of validation failure
            var medications = await _context.Medications
                .Where(m => m.DeletionStatus == MedicationStatus.Active)
                .Select(m => new
                {
                    m.MedicationId,
                    m.Name
                })
                .ToListAsync();

            ViewBag.MedicationList = new SelectList(medications, "MedicationId", "Name", model.MedicationId);

            return View(model);
        }

        // GET: Nurse/DeleteNonScheduledMedication/5
        public async Task<IActionResult> DeleteNonScheduledMedication(int id)
        {
            ViewData["ShowSidebar"] = true;
            var nonScheduledMedication = await _context.NonScheduledMedications
                .Include(nsm => nsm.Medication)
                .FirstOrDefaultAsync(nsm => nsm.Id == id && nsm.AdministeredBy == User.Identity.Name); // Ensure it's the nurse's own record

            if (nonScheduledMedication == null)
            {
                return NotFound();
            }

            var model = new NonScheduledMedicationViewModel
            {
                Id = nonScheduledMedication.Id,
                MedicationName = nonScheduledMedication.Medication.Name,
                Dosage = nonScheduledMedication.Dosage,
                AdministeredDate = nonScheduledMedication.AdministeredDate,
                AdministeredBy = nonScheduledMedication.AdministeredBy
            };

            return View(model);
        }

        // POST: Nurse/DeleteNonScheduledMedication/5
        [HttpPost, ActionName("DeleteNonScheduledMedication")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedNonScheduledMedication(int id)
        {
            var nonScheduledMedication = await _context.NonScheduledMedications
                .FirstOrDefaultAsync(nsm => nsm.Id == id && nsm.AdministeredBy == User.Identity.Name); // Ensure it's the nurse's own record

            if (nonScheduledMedication == null)
            {
                return NotFound();
            }

            // Set the status to Deleted instead of removing the record
            nonScheduledMedication.Status = NonScheduledMedicationStatus.Deleted;
            _context.Update(nonScheduledMedication);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Non-scheduled medication marked as deleted successfully.";
            TempData["MessageType"] = "success";

            return RedirectToAction("ListNonScheduledMedications");
        }













































        public async Task<IActionResult> RequestAdvice()
        {
            ViewData["ShowSidebar"] = true;

            // Get the list of doctors
            var doctors = await _context.Users
                .Where(u => u.Role == ApplicationUser.UserRole.DOCTOR) // Enum value for Doctor role
                .Select(u => new
                {
                    u.Id,
                    FullName = $"{u.FirstName} {u.LastName}"
                })
                .ToListAsync();

            // Get the list of admitted patients
            var admittedPatients = await _context.AdmitPatients
                .Where(ap => ap.AdmitPatientStatus == AdmitPatientStatus.Admitted)
                .Select(ap => new
                {
                    ap.PatientId,
                    FullName = $"{ap.Patient.FirstName} {ap.Patient.LastName}"
                })
                .ToListAsync();

            var model = new DoctorAdviceRequestViewModel
            {
                RequestDate = DateTime.Now,
                DoctorList = new SelectList(doctors, "Id", "FullName"),
                PatientList = new SelectList(admittedPatients, "PatientId", "FullName")
            };

            return View(model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestAdvice(DoctorAdviceRequestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please correct the errors in the form.";
                return View(model);
            }

            if (model.NurseId != null && !await _context.Users.AnyAsync(u => u.Id == model.NurseId))
            {
                TempData["ErrorMessage"] = "The selected nurse does not exist.";
                return View(model);
            }

            if (!await _context.Users.AnyAsync(u => u.Id == model.DoctorId))
            {
                TempData["ErrorMessage"] = "The selected doctor does not exist.";
                return View(model);
            }

            if (!await _context.Users.AnyAsync(u => u.Id == model.PatientId))
            {
                TempData["ErrorMessage"] = "The selected patient does not exist.";
                return View(model);
            }

            var request = new DoctorAdviceRequest
            {
                NurseId = model.NurseId,
                DoctorId = model.DoctorId,
                PatientId = model.PatientId,
                RequestDetails = model.RequestDetails,
                RequestDate = model.RequestDate,
                Status = DoctorAdviceRequestStatus.Pending
            };

            _context.DoctorAdviceRequests.Add(request);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Advice request submitted successfully.";
            return RedirectToAction("ListAdviceRequests"); // Or another appropriate action
        }

        public async Task<IActionResult> ListAdviceRequests()
        {
            ViewData["ShowSidebar"] = true;

            var requests = await _context.DoctorAdviceRequests
     .Where(r => r.Status != DoctorAdviceRequestStatus.Deleted)
     .Include(r => r.Nurse) // Ensure these properties exist in your DbContext
     .Include(r => r.Doctor)
     .Include(r => r.Patient)
     .ToListAsync();


            var model = requests.Select(r => new DoctorAdviceRequestListViewModel
            {
                Id = r.Id,
                NurseName = r.Nurse != null ? $"{r.Nurse.FirstName} {r.Nurse.LastName}" : "N/A",
                DoctorName = r.Doctor != null ? $"{r.Doctor.FirstName} {r.Doctor.LastName}" : "N/A",
                PatientName = r.Patient != null ? $"{r.Patient.FirstName} {r.Patient.LastName}" : "N/A",
                RequestDetails = r.RequestDetails,
                RequestDate = r.RequestDate,
                Status = r.Status
            }).ToList();


            return View(model);
        }






        // GET: Edit Advice Request
        public async Task<IActionResult> EditAdviceRequest(int id)
        {
            ViewData["ShowSidebar"] = true;

            // Retrieve the existing request
            var request = await _context.DoctorAdviceRequests
                .Include(r => r.Nurse)
                .Include(r => r.Doctor)
                .Include(r => r.Patient)
                .FirstOrDefaultAsync(r => r.Id == id && r.Status != DoctorAdviceRequestStatus.Deleted);

            if (request == null)
            {
                TempData["ErrorMessage"] = "The requested advice does not exist.";
                return NotFound();
            }

            // Get the list of doctors
            var doctors = await _context.Users
                .Where(u => u.Role == ApplicationUser.UserRole.DOCTOR)
                .Select(u => new
                {
                    u.Id,
                    FullName = $"{u.FirstName} {u.LastName}"
                })
                .ToListAsync();

            // Get the list of admitted patients
            var admittedPatients = await _context.AdmitPatients
                .Where(ap => ap.AdmitPatientStatus == AdmitPatientStatus.Admitted)
                .Select(ap => new
                {
                    ap.PatientId,
                    FullName = $"{ap.Patient.FirstName} {ap.Patient.LastName}"
                })
                .ToListAsync();

            var model = new DoctorAdviceRequestViewModel
            {
                Id = request.Id,
                NurseId = request.NurseId,
                DoctorId = request.DoctorId,
                PatientId = request.PatientId,
                RequestDetails = request.RequestDetails,
                RequestDate = request.RequestDate,
                DoctorList = new SelectList(doctors, "Id", "FullName", request.DoctorId),
                PatientList = new SelectList(admittedPatients, "PatientId", "FullName", request.PatientId)
            };

            return View(model);
        }

        // POST: Edit Advice Request
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAdviceRequest(DoctorAdviceRequestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please correct the errors in the form.";
                return View(model);
            }

            // Retrieve the existing request
            var request = await _context.DoctorAdviceRequests.FindAsync(model.Id);
            if (request == null || request.Status == DoctorAdviceRequestStatus.Deleted)
            {
                TempData["ErrorMessage"] = "The requested advice does not exist.";
                return NotFound();
            }

            // Update the request properties
            request.DoctorId = model.DoctorId;
            request.PatientId = model.PatientId;
            request.RequestDetails = model.RequestDetails;
            request.RequestDate = model.RequestDate;

            try
            {
                _context.Update(request);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Advice request updated successfully.";
                return RedirectToAction("ListAdviceRequests");
            }
            catch (DbUpdateConcurrencyException)
            {
                TempData["ErrorMessage"] = "An error occurred while updating the request. Please try again.";
                // Log the error or handle it as needed
            }

            return View(model);
        }







        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsCompleted(int id)
        {
            // Retrieve the request by ID
            var request = await _context.DoctorAdviceRequests
                .FirstOrDefaultAsync(r => r.Id == id);

            // Check if the request exists
            if (request == null)
            {
                TempData["ErrorMessage"] = "The requested advice does not exist.";
                return RedirectToAction("ListAdviceRequests");
            }

            // Check if the status is already completed
            if (request.Status == DoctorAdviceRequestStatus.Completed)
            {
                TempData["ErrorMessage"] = "The request is already marked as completed.";
                return RedirectToAction("ListAdviceRequests");
            }

            // Update the status to Completed
            request.Status = DoctorAdviceRequestStatus.Completed;

            try
            {
                // Save the changes
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Advice request marked as completed successfully.";
            }
            catch (DbUpdateException ex)
            {
                TempData["ErrorMessage"] = "An error occurred while marking the request as completed. Please try again.";
                // Log the exception for diagnostics
                // You can use a logging library like NLog, Serilog, etc. 
                // _logger.LogError(ex, "Error occurred while marking advice request {RequestId} as completed", id);
            }

            // Redirect to the list of advice requests
            return RedirectToAction("ListAdviceRequests");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRequest(int id)
        {
            var currentUserName = User.Identity.Name;

            // Retrieve the request by ID, ensure it's created by the current nurse
            var request = await _context.DoctorAdviceRequests
                .FirstOrDefaultAsync(r => r.Id == id
                                          && r.Nurse.UserName == currentUserName); // Check ownership

            if (request == null)
            {
                TempData["ErrorMessage"] = "The requested advice does not exist.";
                return RedirectToAction("ListAdviceRequests");
            }

            // Update the request status to Deleted
            request.Status = DoctorAdviceRequestStatus.Deleted;

            try
            {
                // Mark the entity as modified
                _context.DoctorAdviceRequests.Update(request);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Advice request deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the request. Please try again.";
                // Optionally log the exception here
            }

            // Redirect to the list of advice requests
            return RedirectToAction("ListAdviceRequests");
        }























        public async Task<IActionResult> SelectPatient()
        {
            ViewData["ShowSidebar"] = true;
            // Fetch all admitted patients
            var admittedPatients = await _context.AdmitPatients
                .Where(ap => ap.AdmitPatientStatus == AdmitPatientStatus.Admitted)
                .Select(ap => new SelectListItem
                {
                    Value = ap.PatientId, // PatientId from the admitted patient
                    Text = ap.Patient.FirstName + " " + ap.Patient.LastName
                })
                .ToListAsync();

            // Pass the patients to the view via ViewBag
            ViewBag.Patients = admittedPatients;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PatientHistory(string patientId)
        {
            ViewData["ShowSidebar"] = true;
            if (string.IsNullOrEmpty(patientId))
            {
                return RedirectToAction("SelectPatient");
            }

            var patient = await _context.Users
                .Where(u => u.Id == patientId)
                .Select(u => new
                {
                    u.FirstName,
                    u.LastName
                })
                .FirstOrDefaultAsync();

            if (patient == null)
            {
                return NotFound("Patient not found.");
            }

            // Fetch history as explained earlier
            var history = new PatientHistoryViewModel
            {
                FullName = $"{patient.FirstName} {patient.LastName}",

                Admissions = await _context.AdmitPatients
                    .Where(ap => ap.PatientId == patientId)
                    .Select(ap => new AdmissionHistory
                    {
                        AdmissionDate = ap.AdmissionDate,
                        Status = ap.AdmitPatientStatus
                    })
                    .ToListAsync(),

                Allergies = await _context.PatientAllergies
                    .Where(pa => pa.PatientId == patientId)
                    .Include(pa => pa.Allergy)
                    .ToListAsync(),

                Medications = await _context.PatientMedications
                    .Where(pm => pm.PatientId == patientId)
                    .Include(pm => pm.Medication)
                    .ToListAsync(),

                Conditions = await _context.PatientConditions
                    .Where(pc => pc.PatientId == patientId)
                    .Include(pc => pc.Condition)
                    .ToListAsync(),

                Treatments = await _context.PatientTreatments
                    .Where(pt => pt.PatientId == patientId && pt.TreatmentStatus == TreatmentStatus.Active)
                    .ToListAsync(),

                Vitals = await _context.PatientVitals
                    .Where(pv => pv.PatientId == patientId && pv.PatientVitalStatus == PatientVitalStatus.Active)
                    .ToListAsync()
            };

            return View(history);
        }





























        private bool PatientVitalExists(int id)
        {
            return _context.PatientVitals.Any(e => e.Id == id);
        }
        private bool PatientTreatmentExists(int id)
        {
            return _context.PatientTreatments.Any(t => t.Id == id);
        }
        private bool NonScheduledMedicationExists(int id)
        {
            return _context.NonScheduledMedications.Any(nsm => nsm.Id == id);
        }
      

    }
}
