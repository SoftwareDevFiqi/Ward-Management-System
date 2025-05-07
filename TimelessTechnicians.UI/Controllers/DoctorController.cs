using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System.Security.Claims;
using TimelessTechnicians.UI.Data;
using TimelessTechnicians.UI.Models;
using TimelessTechnicians.UI.ViewModel;
using static TimelessTechnicians.UI.Models.ApplicationUser;

namespace TimelessTechnicians.UI.Controllers
{
    [Authorize(Roles = "DOCTOR,PATIENT,NURSE")]
    public class DoctorController : Controller
    {

        private readonly ApplicationDbContext _context;

        public DoctorController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> DoctorDashboard()
        {
            var currentUserName = User.Identity.Name;
            ViewData["ShowSidebar"] = true;

            // Fetch statistics
            var totalPrescriptions = await _context.Prescriptions.CountAsync();
            var totalPatients = await _context.AdmitPatients.CountAsync(); // Assuming you have a table for admitted patients

            // Fetch prescription status counts
            var totalForwarded = await _context.Prescriptions.CountAsync(p => p.Status == InstructionStatus.ForwardedToPharmacy);
            var totalDelivered = await _context.Prescriptions.CountAsync(p => p.Status == InstructionStatus.Delivered);
            var totalReceived = await _context.Prescriptions.CountAsync(p => p.Status == InstructionStatus.Received);

            // Fetch total appointments
            var totalAppointments = await _context.PatientAppointments.CountAsync(ap => ap.Status == AppointmentStatus.Scheduled);

            // Store statistics in ViewData
            ViewData["TotalPrescriptions"] = totalPrescriptions;
            ViewData["TotalPatients"] = totalPatients; // Total number of patients for the doctor
            ViewData["TotalForwarded"] = totalForwarded;
            ViewData["TotalDelivered"] = totalDelivered;
            ViewData["TotalReceived"] = totalReceived;
            ViewData["TotalAppointments"] = totalAppointments; // Total scheduled appointments

            return View();
        }








        public async Task<IActionResult> ListPatientInstructions()
        {
            ViewData["ShowSidebar"] = true;

            // Get the current user's name (the doctor or nurse)
            var currentUserName = User.Identity.Name;
            var currentUserRole = User.IsInRole(UserRole.DOCTOR.ToString())
                                   ? UserRole.DOCTOR
                                   : UserRole.NURSE; // Assuming the user can only be a doctor or a nurse

            // Retrieve the patient instructions based on the user's role
            IQueryable<PatientInstruction> patientInstructions;

            if (currentUserRole == UserRole.DOCTOR)
            {
                // Doctor sees their own instructions
                patientInstructions = _context.PatientInstructions
                    .Where(pi => pi.AdministeredBy == currentUserName && pi.Status != InstructionStatus.Deleted)
                    .Include(pi => pi.AdmitPatient)
                        .ThenInclude(ap => ap.Patient);
            }
            else if (currentUserRole == UserRole.NURSE)
            {
                // Nurse sees instructions by doctors (not deleted)
                patientInstructions = _context.PatientInstructions
                    .Where(pi => pi.Status != InstructionStatus.Deleted)
                    .Include(pi => pi.AdmitPatient)
                        .ThenInclude(ap => ap.Patient);
            }
            else
            {
                // Return a view with no instructions if the role is not doctor or nurse
                patientInstructions = Enumerable.Empty<PatientInstruction>().AsQueryable();
            }

            var instructionsList = await patientInstructions.ToListAsync();
            return View(instructionsList);
        }





        public IActionResult AddPatientInstruction()
        {
            ViewData["ShowSidebar"] = true;
            PopulateAdmittedPatientsDropdown(); // Populate dropdown list for admitted patients
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPatientInstruction([Bind("AdmitPatientId,Instruction")] PatientInstruction patientInstruction)
        {
            var currentUserName = User.Identity.Name;

            var existingInstruction = await _context.PatientInstructions
                .FirstOrDefaultAsync(pi => pi.AdmitPatientId == patientInstruction.AdmitPatientId && pi.AdministeredBy != currentUserName && pi.Status != InstructionStatus.Deleted);

            if (existingInstruction != null)
            {
                TempData["ErrorMessage"] = "This patient is already assigned to another doctor.";
                PopulateAdmittedPatientsDropdown(); // Re-populate dropdown list for admitted patients
                return View(patientInstruction);
            }

            patientInstruction.AdministeredBy = currentUserName;
            patientInstruction.Status = InstructionStatus.Active;

            _context.Add(patientInstruction);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Patient instruction added successfully!";
            return RedirectToAction(nameof(ListPatientInstructions), new { id = patientInstruction.Id });
        }

        // GET: Edit Patient Instruction
        public async Task<IActionResult> EditPatientInstruction(int id)
        {
            ViewData["ShowSidebar"] = true;

            var currentUserName = User.Identity.Name; // Get the current user's name
            var patientInstruction = await _context.PatientInstructions
                .Include(pi => pi.AdmitPatient) // Include the AdmitPatient to access its properties
                    .ThenInclude(ap => ap.Patient) // Include the Patient entity
                .FirstOrDefaultAsync(pi => pi.Id == id && pi.AdministeredBy == currentUserName && pi.Status != InstructionStatus.Deleted);

            if (patientInstruction == null)
            {
                return NotFound();
            }

            // Populate the dropdown with the selected patient
            PopulateAdmittedPatientsDropdown(patientInstruction.AdmitPatientId);
            return View(patientInstruction);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPatientInstruction(int id, [Bind("Id,AdmitPatientId,Instruction,DateIssued,Status")] PatientInstruction patientInstruction)
        {
            if (id != patientInstruction.Id)
            {
                return NotFound();
            }

            var currentUserName = User.Identity.Name;
            var existingInstruction = await _context.PatientInstructions
                .FirstOrDefaultAsync(pi => pi.Id == id && pi.AdministeredBy == currentUserName && pi.Status != InstructionStatus.Deleted);

            if (existingInstruction == null)
            {
                TempData["ErrorMessage"] = "Instruction not found or does not belong to the current user.";
                return NotFound();
            }

            // Check for patient conflict
            if (existingInstruction.AdmitPatientId != patientInstruction.AdmitPatientId)
            {
                var patientConflict = await _context.PatientInstructions
                    .FirstOrDefaultAsync(pi => pi.AdmitPatientId == patientInstruction.AdmitPatientId && pi.AdministeredBy != currentUserName && pi.Status != InstructionStatus.Deleted);

                if (patientConflict != null)
                {
                    TempData["ErrorMessage"] = "This patient is already assigned to another doctor.";
                    PopulateAdmittedPatientsDropdown(patientInstruction.AdmitPatientId);
                    return View(existingInstruction);
                }
            }

            try
            {
                existingInstruction.AdmitPatientId = patientInstruction.AdmitPatientId;
                existingInstruction.Instruction = patientInstruction.Instruction;
                existingInstruction.DateIssued = patientInstruction.DateIssued;

                _context.Update(existingInstruction);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PatientInstructionExists(patientInstruction.Id))
                {
                    return NotFound();
                }
                throw;
            }

            TempData["SuccessMessage"] = "Patient instruction updated successfully!";
            return RedirectToAction(nameof(ListPatientInstructions), new { id = patientInstruction.Id });
        }



        // GET: Delete Patient Instruction
        public async Task<IActionResult> DeletePatientInstruction(int id)
        {
            ViewData["ShowSidebar"] = true;

            var currentUserName = User.Identity.Name; // Get the current user's name
            var patientInstruction = await _context.PatientInstructions
                .Include(pi => pi.AdmitPatient) // Include the AdmitPatient to access its properties
                    .ThenInclude(ap => ap.Patient) // Include the Patient entity
                .FirstOrDefaultAsync(pi => pi.Id == id && pi.AdministeredBy == currentUserName && pi.Status != InstructionStatus.Deleted);

            if (patientInstruction == null)
            {
                return NotFound();
            }

            return View(patientInstruction);
        }


        // POST: PatientInstructions/Delete/5
        [HttpPost, ActionName("DeletePatientInstruction")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePatientInstructionConfirmed(int id)
        {
            var currentUserName = User.Identity.Name;
            var patientInstruction = await _context.PatientInstructions
                .FirstOrDefaultAsync(pi => pi.Id == id && pi.AdministeredBy == currentUserName && pi.Status != InstructionStatus.Deleted);

            if (patientInstruction == null)
            {
                TempData["ErrorMessage"] = "Instruction not found or does not belong to the current user.";
                return NotFound();
            }

            patientInstruction.Status = InstructionStatus.Deleted;
            _context.Update(patientInstruction);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Patient instruction deleted successfully.";
            return RedirectToAction(nameof(ListPatientInstructions));
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



        public IActionResult AddPatientDischarge()
        {
            ViewData["ShowSidebar"] = true;
            PopulateAdmittedPatientsDropdown(); // Populate dropdown for admitted patients
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPatientDischarge([Bind("AdmitPatientId,DischargeReason,DischargeDate")] PatientDischarge patientDischarge)
        {
            var currentUserName = User.Identity.Name;

            // Check if the patient has already been discharged
            var existingDischarge = await _context.PatientDischargeInstructions
                .FirstOrDefaultAsync(pd => pd.AdmitPatientId == patientDischarge.AdmitPatientId
                    && pd.AdministeredBy == currentUserName
                    && pd.Status != InstructionStatus.Deleted);

            if (existingDischarge != null)
            {
                TempData["ErrorMessage"] = "You have already discharged this patient.";
                PopulateAdmittedPatientsDropdown();
                return View(patientDischarge);
            }

            // Check if there is any active discharge instruction for the patient
            var dischargedPatient = await _context.PatientDischargeInstructions
                .FirstOrDefaultAsync(pd => pd.AdmitPatientId == patientDischarge.AdmitPatientId
                    && pd.Status != InstructionStatus.Deleted);

            if (dischargedPatient != null)
            {
                TempData["ErrorMessage"] = "This patient has already been discharged.";
                PopulateAdmittedPatientsDropdown();
                return View(patientDischarge);
            }

            // Set the administered by user and default Notes
            patientDischarge.AdministeredBy = currentUserName;
            patientDischarge.Status = InstructionStatus.Active;
            patientDischarge.Notes = string.Empty; // Set a default value for Notes

            // Add discharge record to the context and save
            _context.Add(patientDischarge);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Patient discharge added successfully.";
            return RedirectToAction(nameof(ListPatientDischarges));
        }

        public async Task<IActionResult> EditPatientDischarge(int id)
        {
            ViewData["ShowSidebar"] = true;

            var currentUserName = User.Identity.Name;
            var patientDischarge = await _context.PatientDischargeInstructions
                .Include(pd => pd.AdmitPatient)
                    .ThenInclude(ap => ap.Patient)
                .FirstOrDefaultAsync(pd => pd.Id == id && pd.AdministeredBy == currentUserName && pd.Status != InstructionStatus.Deleted);

            if (patientDischarge == null)
            {
                return NotFound();
            }

            // Show the patient's full name at the top of the edit page
            ViewData["PatientName"] = $"{patientDischarge.AdmitPatient.Patient.FirstName} {patientDischarge.AdmitPatient.Patient.LastName}";

            PopulateAdmittedPatientsDropdown(patientDischarge.AdmitPatientId);
            return View(patientDischarge);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPatientDischarge(int id, [Bind("Id,AdmitPatientId,DischargeReason,DischargeDate,Status")] PatientDischarge patientDischarge)
        {
            if (id != patientDischarge.Id)
            {
                return NotFound();
            }

            var currentUserName = User.Identity.Name;
            var existingDischarge = await _context.PatientDischargeInstructions
                .FirstOrDefaultAsync(pd => pd.Id == id && pd.AdministeredBy == currentUserName && pd.Status != InstructionStatus.Deleted);

            if (existingDischarge == null)
            {
                TempData["ErrorMessage"] = "Discharge record not found or does not belong to the current user.";
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(patientDischarge.DischargeReason) || patientDischarge.DischargeDate == default)
            {
                ModelState.AddModelError("", "Discharge Reason and Date must be provided.");
                PopulateAdmittedPatientsDropdown(existingDischarge.AdmitPatientId);
                return View(existingDischarge);
            }

            existingDischarge.DischargeReason = patientDischarge.DischargeReason;
            existingDischarge.DischargeDate = patientDischarge.DischargeDate;

            try
            {
                _context.Update(existingDischarge);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Patient discharge updated successfully."; // Success message
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PatientDischargeExists(patientDischarge.Id))
                {
                    return NotFound();
                }
                throw;
            }

            return RedirectToAction(nameof(ListPatientDischarges));
        }

        public async Task<IActionResult> DeletePatientDischarge(int id)
        {
            ViewData["ShowSidebar"] = true;

            var currentUserName = User.Identity.Name;
            var patientDischarge = await _context.PatientDischargeInstructions
                .Include(pd => pd.AdmitPatient)
                    .ThenInclude(ap => ap.Patient)
                .FirstOrDefaultAsync(pd => pd.Id == id && pd.AdministeredBy == currentUserName && pd.Status != InstructionStatus.Deleted);

            if (patientDischarge == null)
            {
                return NotFound();
            }

            return View(patientDischarge);
        }

        [HttpPost, ActionName("DeletePatientDischarge")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePatientDischargeConfirmed(int id)
        {
            var currentUserName = User.Identity.Name;
            var patientDischarge = await _context.PatientDischargeInstructions
                .FirstOrDefaultAsync(pd => pd.Id == id && pd.AdministeredBy == currentUserName && pd.Status != InstructionStatus.Deleted);

            if (patientDischarge == null)
            {
                TempData["ErrorMessage"] = "Discharge record not found or does not belong to the current user.";
                return NotFound();
            }

            patientDischarge.Status = InstructionStatus.Deleted;
            _context.Update(patientDischarge);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Patient discharge record deleted successfully."; // Success message
            return RedirectToAction(nameof(ListPatientDischarges));
        }














        public async Task<IActionResult> ListMedicationPrescriptions()
        {
            ViewData["ShowSidebar"] = true;
            var currentUserName = User.Identity.Name;

            var medicationPrescriptions = await _context.MedicationPrescriptions
                .Where(mp => mp.AdministeredBy == currentUserName && mp.Status != MedicationStatus.Deleted)
                .Include(mp => mp.AdmitPatient)
                    .ThenInclude(ap => ap.Patient)
                .Include(mp => mp.Medication)
                .ToListAsync();

            return View(medicationPrescriptions);
        }

        public IActionResult AddMedicationPrescription()
        {
            ViewData["ShowSidebar"] = true;
            PopulateAdmittedPatientsDropdown();
            PopulateMedicationsDropdown(); // Ensure medications are populated
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMedicationPrescription([Bind("AdmitPatientId,MedicationId,Dosage")] MedicationPrescription medicationPrescription)
        {
            var currentUserName = User.Identity.Name;

            medicationPrescription.AdministeredBy = currentUserName;
            medicationPrescription.DatePrescribed = DateTime.Now;
            medicationPrescription.Status = MedicationStatus.Active;

            _context.Add(medicationPrescription);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Medication prescription added successfully."; // Success message
            return RedirectToAction(nameof(ListMedicationPrescriptions));
        }

        // GET: Edit MedicationPrescription
        public async Task<IActionResult> EditMedicationPrescription(int id)
        {
            ViewData["ShowSidebar"] = true;

            var medicationPrescription = await _context.MedicationPrescriptions
                .Include(mp => mp.AdmitPatient)
                .Include(mp => mp.Medication)
                .FirstOrDefaultAsync(mp => mp.Id == id);

            if (medicationPrescription == null)
            {
                TempData["ErrorMessage"] = "Medication prescription not found."; // Error message
                return NotFound();
            }

            PopulateAdmittedPatientsDropdown(medicationPrescription.AdmitPatientId);
            PopulateMedicationsDropdown(medicationPrescription.MedicationId);

            return View(medicationPrescription);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMedicationPrescription(int id, [Bind("Id,AdmitPatientId,MedicationId,Dosage,DatePrescribed,Status")] MedicationPrescription medicationPrescription)
        {
            if (id != medicationPrescription.Id)
            {
                TempData["ErrorMessage"] = "Invalid medication prescription."; // Error message
                return NotFound();
            }

            try
            {
                var existingPrescription = await _context.MedicationPrescriptions.FindAsync(id);
                if (existingPrescription == null)
                {
                    TempData["ErrorMessage"] = "Medication prescription not found."; // Error message
                    return NotFound();
                }

                existingPrescription.AdmitPatientId = medicationPrescription.AdmitPatientId;
                existingPrescription.MedicationId = medicationPrescription.MedicationId;
                existingPrescription.Dosage = medicationPrescription.Dosage;

                _context.Update(existingPrescription);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Medication prescription updated successfully."; // Success message
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MedicationPrescriptionExists(medicationPrescription.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(ListMedicationPrescriptions));
        }

        // GET: Delete MedicationPrescription
        public async Task<IActionResult> DeleteMedicationPrescription(int id)
        {
            ViewData["ShowSidebar"] = true;

            var currentUserName = User.Identity.Name;
            var medicationPrescription = await _context.MedicationPrescriptions
                .Include(mp => mp.AdmitPatient)
                    .ThenInclude(ap => ap.Patient)
                .Include(mp => mp.Medication)
                .FirstOrDefaultAsync(mp => mp.Id == id && mp.AdministeredBy == currentUserName && mp.Status != MedicationStatus.Deleted);

            if (medicationPrescription == null)
            {
                TempData["ErrorMessage"] = "Medication prescription not found."; // Error message
                return NotFound();
            }

            return View(medicationPrescription);
        }

        [HttpPost, ActionName("DeleteMedicationPrescription")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMedicationPrescriptionConfirmed(int id)
        {
            var currentUserName = User.Identity.Name;
            var medicationPrescription = await _context.MedicationPrescriptions
                .FirstOrDefaultAsync(mp => mp.Id == id && mp.AdministeredBy == currentUserName && mp.Status != MedicationStatus.Deleted);

            if (medicationPrescription == null)
            {
                TempData["ErrorMessage"] = "Medication prescription not found."; // Error message
                return NotFound();
            }

            medicationPrescription.Status = MedicationStatus.Deleted;
            _context.Update(medicationPrescription);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Medication prescription deleted successfully."; // Success message
            return RedirectToAction(nameof(ListMedicationPrescriptions));
        }





        public async Task<IActionResult> ListVisitSchedules()
        {
            ViewData["ShowSidebar"] = true;

            var visitSchedules = await _context.PatientVisitSchedules
                .Include(vs => vs.AdmitPatient)
                .ThenInclude(ap => ap.Patient)
                .Where(vs => vs.Status == VisitStatus.Scheduled)
                .ToListAsync();

            return View(visitSchedules);
        }

        public IActionResult AddVisitSchedule()
        {
            ViewData["ShowSidebar"] = true;

            // Get the list of admitted patients for the dropdown
            ViewBag.AdmittedPatients = new SelectList(
                _context.AdmitPatients
                    .Where(ap => ap.AdmitPatientStatus == AdmitPatientStatus.Admitted)
                    .Select(ap => new { ap.Id, FullName = $"{ap.Patient.FirstName} {ap.Patient.LastName}" }),
                "Id", "FullName");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddVisitSchedule([Bind("AdmitPatientId,ScheduledDate,VisitReason")] PatientVisitSchedule visitSchedule)
        {
                visitSchedule.Status = VisitStatus.Scheduled; // Ensure the visit is marked as scheduled
                _context.Add(visitSchedule);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Visit schedule added successfully.";
                return RedirectToAction(nameof(ListVisitSchedules));
            

            // Repopulate the dropdown list in case of validation errors
            ViewBag.AdmittedPatients = new SelectList(
                _context.AdmitPatients.Where(ap => ap.AdmitPatientStatus == AdmitPatientStatus.Admitted)
                    .Select(ap => new { ap.Id, FullName = $"{ap.Patient.FirstName} {ap.Patient.LastName}" }),
                "Id", "FullName", visitSchedule.AdmitPatientId);

            ViewData["ShowSidebar"] = true;
            TempData["ErrorMessage"] = "Please correct the errors in the form.";
            return View(visitSchedule);
        }

        public async Task<IActionResult> EditVisitSchedule(int id)
        {
            ViewData["ShowSidebar"] = true;

            var visitSchedule = await _context.PatientVisitSchedules.FindAsync(id);
            if (visitSchedule == null)
            {
                TempData["ErrorMessage"] = "The requested visit schedule does not exist.";
                return NotFound();
            }

            // Populate admitted patients dropdown for editing
            ViewBag.AdmittedPatients = new SelectList(
                _context.AdmitPatients.Where(ap => ap.AdmitPatientStatus == AdmitPatientStatus.Admitted)
                    .Select(ap => new { ap.Id, FullName = $"{ap.Patient.FirstName} {ap.Patient.LastName}" }),
                "Id", "FullName", visitSchedule.AdmitPatientId);

            return View(visitSchedule);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditVisitSchedule(int id, [Bind("Id,AdmitPatientId,ScheduledDate,VisitReason,Status")] PatientVisitSchedule visitSchedule)
        {
            if (id != visitSchedule.Id)
            {
                TempData["ErrorMessage"] = "Visit schedule ID mismatch.";
                return NotFound();
            }

           
                try
                {
                    _context.Update(visitSchedule);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Visit schedule updated successfully.";
                    return RedirectToAction(nameof(ListVisitSchedules));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.PatientVisitSchedules.Any(vs => vs.Id == id))
                    {
                        TempData["ErrorMessage"] = "The requested visit schedule does not exist.";
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            

            // Repopulate the patient information in case of validation error
            ViewBag.AdmittedPatients = new SelectList(
                _context.AdmitPatients.Where(ap => ap.AdmitPatientStatus == AdmitPatientStatus.Admitted)
                    .Select(ap => new { ap.Id, FullName = $"{ap.Patient.FirstName} {ap.Patient.LastName}" }),
                "Id", "FullName", visitSchedule.AdmitPatientId);

            ViewData["ShowSidebar"] = true;
            TempData["ErrorMessage"] = "Please correct the errors in the form.";
            return View(visitSchedule);
        }

        public async Task<IActionResult> DeleteVisitSchedule(int id)
        {
            ViewData["ShowSidebar"] = true;

            var visitSchedule = await _context.PatientVisitSchedules
                .Include(vs => vs.AdmitPatient)
                .ThenInclude(ap => ap.Patient)
                .FirstOrDefaultAsync(vs => vs.Id == id);

            if (visitSchedule == null)
            {
                TempData["ErrorMessage"] = "The requested visit schedule does not exist.";
                return NotFound();
            }

            return View(visitSchedule);
        }

        [HttpPost, ActionName("DeleteVisitSchedule")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteVisitScheduleConfirmed(int id)
        {
            var visitSchedule = await _context.PatientVisitSchedules.FindAsync(id);
            if (visitSchedule != null)
            {
                // Set the status to Cancelled to indicate it's "deleted"
                visitSchedule.Status = VisitStatus.Cancelled; // Assuming "Cancelled" indicates a soft delete
                _context.PatientVisitSchedules.Update(visitSchedule);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Visit schedule deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "The requested visit schedule does not exist.";
            }

            return RedirectToAction(nameof(ListVisitSchedules));
        }
























































































        // GET: Appointments
        public async Task<IActionResult> ListAppointments()
        {
            ViewData["ShowSidebar"] = true;

            var currentUserName = User.Identity.Name; // Get the logged-in user's identity
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value; // Assuming roles are stored in claims

            IQueryable<PatientAppointment> appointmentsQuery = _context.PatientAppointments
                .Include(ap => ap.AdmitPatient)
                .ThenInclude(ap => ap.Patient)
                .Include(ap => ap.Doctor); // Include doctor information

            // Check if the user is a doctor or a patient and filter appointments accordingly
            if (currentUserRole == UserRole.DOCTOR.ToString())
            {
                // If the user is a doctor, show all appointments they are associated with
                appointmentsQuery = appointmentsQuery.Where(ap => ap.Doctor.UserName == currentUserName && ap.Status == AppointmentStatus.Scheduled);
            }
            else if (currentUserRole == UserRole.PATIENT.ToString())
            {
                // If the user is a patient, show only their own appointments
                appointmentsQuery = appointmentsQuery.Where(ap => ap.AdmitPatient.Patient.UserName == currentUserName && ap.Status == AppointmentStatus.Scheduled);
            }
            else
            {
                // If neither, return a forbidden response or empty view
                return Forbid();
            }

            var appointments = await appointmentsQuery
                .Select(ap => new AppointmentListViewModel // Assuming you have a ViewModel for appointments
                {
                    AppointmentId = ap.Id,
                    PatientName = $"{ap.AdmitPatient.Patient.FirstName} {ap.AdmitPatient.Patient.LastName}", // Full name of the patient
                    AppointmentDate = ap.AppointmentDate,
                    Status = ap.Status.ToString(),
                    DoctorName = $"{ap.Doctor.FirstName} {ap.Doctor.LastName}" // Full name of the doctor
                })
                .ToListAsync();

            return View(appointments);
        }






        // GET: AddAppointment
        public IActionResult AddAppointment()
        {
            ViewData["ShowSidebar"] = true;

            // Get the list of admitted patients for the dropdown
            ViewBag.AdmittedPatients = new SelectList(
                _context.AdmitPatients
                    .Where(ap => ap.AdmitPatientStatus == AdmitPatientStatus.Admitted)
                    .Select(ap => new { ap.Id, FullName = $"{ap.Patient.FirstName} {ap.Patient.LastName}" }),
                "Id", "FullName");

            return View();
        }

        // POST: AddAppointment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAppointment([Bind("AdmitPatientId,AppointmentDate,Reason")] PatientAppointment appointment)
        {
            // Retrieve logged-in doctor's ID from HttpContext.User
            var doctorIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "DoctorId");
            if (doctorIdClaim == null)
            {
                TempData["ErrorMessage"] = "Doctor information not found. Please log in.";
                return RedirectToAction(nameof(AddAppointment)); // Redirect to appointments list or login page
            }

            appointment.DoctorId = doctorIdClaim.Value; // Associate the logged-in doctor as a string

           
                appointment.Status = AppointmentStatus.Scheduled; // Mark the appointment as scheduled
                _context.Add(appointment);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Appointment added successfully.";
                return RedirectToAction(nameof(ListAppointments));
            

            // Repopulate the dropdown list in case of validation errors
            ViewBag.AdmittedPatients = new SelectList(
                _context.AdmitPatients
                    .Where(ap => ap.AdmitPatientStatus == AdmitPatientStatus.Admitted)
                    .Select(ap => new { ap.Id, FullName = $"{ap.Patient.FirstName} {ap.Patient.LastName}" }),
                "Id", "FullName", appointment.AdmitPatientId);

            ViewData["ShowSidebar"] = true;
            TempData["ErrorMessage"] = "Please correct the errors in the form.";
            return View(appointment);
        }

        // GET: EditAppointment
        public async Task<IActionResult> EditAppointment(int id)
        {
            ViewData["ShowSidebar"] = true;

            var appointment = await _context.PatientAppointments.FindAsync(id);
            if (appointment == null)
            {
                TempData["ErrorMessage"] = "The requested appointment does not exist.";
                return NotFound();
            }

            // Populate admitted patients dropdown for editing
            ViewBag.AdmittedPatients = new SelectList(
                _context.AdmitPatients.Where(ap => ap.AdmitPatientStatus == AdmitPatientStatus.Admitted)
                    .Select(ap => new { ap.Id, FullName = $"{ap.Patient.FirstName} {ap.Patient.LastName}" }),
                "Id", "FullName", appointment.AdmitPatientId);

            return View(appointment);
        }

        // POST: EditAppointment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAppointment(int id, [Bind("Id,AdmitPatientId,AppointmentDate,Reason,Status")] PatientAppointment appointment)
        {
            if (id != appointment.Id)
            {
                TempData["ErrorMessage"] = "Appointment ID mismatch.";
                return NotFound();
            }

            // Retrieve the existing appointment to get DoctorId
            var existingAppointment = await _context.PatientAppointments.FindAsync(id);
            if (existingAppointment == null)
            {
                TempData["ErrorMessage"] = "The requested appointment does not exist.";
                return NotFound();
            }

            // Update properties of the existing appointment
            existingAppointment.AdmitPatientId = appointment.AdmitPatientId;
            existingAppointment.AppointmentDate = appointment.AppointmentDate;
            existingAppointment.Reason = appointment.Reason;
            existingAppointment.Status = appointment.Status; // Ensure this is valid for your logic
            existingAppointment.DoctorId = existingAppointment.DoctorId; // Maintain the original DoctorId

           
                try
                {
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Appointment updated successfully.";
                    return RedirectToAction(nameof(ListAppointments));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.PatientAppointments.Any(ap => ap.Id == id))
                    {
                        TempData["ErrorMessage"] = "The requested appointment does not exist.";
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            

            // Repopulate the patient information in case of validation error
            ViewBag.AdmittedPatients = new SelectList(
                _context.AdmitPatients.Where(ap => ap.AdmitPatientStatus == AdmitPatientStatus.Admitted)
                    .Select(ap => new { ap.Id, FullName = $"{ap.Patient.FirstName} {ap.Patient.LastName}" }),
                "Id", "FullName", existingAppointment.AdmitPatientId);

            ViewData["ShowSidebar"] = true;
            TempData["ErrorMessage"] = "Please correct the errors in the form.";
            return View(existingAppointment);
        }

        // GET: DeleteAppointment
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            ViewData["ShowSidebar"] = true;

            var appointment = await _context.PatientAppointments
                .Include(ap => ap.AdmitPatient)
                .ThenInclude(ap => ap.Patient)
                .FirstOrDefaultAsync(ap => ap.Id == id);

            if (appointment == null)
            {
                TempData["ErrorMessage"] = "The requested appointment does not exist.";
                return NotFound();
            }

            return View(appointment);
        }

        // POST: DeleteAppointment
        [HttpPost, ActionName("DeleteAppointment")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAppointmentConfirmed(int id)
        {
            var appointment = await _context.PatientAppointments.FindAsync(id);
            if (appointment != null)
            {
                // Set the status to Cancelled to indicate it's "deleted"
                appointment.Status = AppointmentStatus.Cancelled;
                _context.PatientAppointments.Update(appointment);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Appointment cancelled successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "The requested appointment does not exist.";
            }

            return RedirectToAction(nameof(ListAppointments));
        }












































        public async Task<IActionResult> ListPrescriptions()
        {
            ViewData["ShowSidebar"] = true;

            // Get the current user's username
            var currentUserName = User.Identity.Name;

            // Find the currently logged-in user
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == currentUserName);
            if (currentUser == null)
            {
                return NotFound("Current user not found.");
            }

            // Get prescriptions for the current doctor only
            var prescriptions = await _context.Prescriptions
                .Include(p => p.Patient)
                .Where(p => p.DoctorId == currentUser.Id) // Filter by DoctorId
                .ToListAsync();

            return View(prescriptions);
        }





        public IActionResult AddPrescription()
        {
            ViewData["ShowSidebar"] = true;
            PopulatePatientsDropdown(); // Method to populate dropdown
            return View();
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPrescription([Bind("PatientId,Medication,Dosage")] Prescription prescription)
        {
            // Ensure the patient is admitted
            var isPatientAdmitted = await _context.AdmitPatients
                .AnyAsync(ap => ap.PatientId == prescription.PatientId && ap.AdmitPatientStatus == AdmitPatientStatus.Admitted);

            if (!isPatientAdmitted)
            {
                ViewData["ShowSidebar"] = true;
                ModelState.AddModelError("PatientId", "The patient must be admitted before a prescription can be created.");
                PopulatePatientsDropdown(); // Re-populate dropdown on error
                return View(prescription);
            }

            // Set the current doctor's ID
            var currentUserName = User.Identity.Name;
            var currentDoctor = await _context.Users.FirstOrDefaultAsync(u => u.UserName == currentUserName);
            if (currentDoctor == null)
            {
                ViewData["ShowSidebar"] = true;
                return NotFound("Current user not found.");
            }

            prescription.DoctorId = currentDoctor.Id;
            prescription.DateWritten = DateTime.Now;
            prescription.Status = InstructionStatus.Active;

            _context.Add(prescription);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ListPrescriptions));
        }


        // GET: EditPrescription
        public async Task<IActionResult> EditPrescription(int id)
        {
            ViewData["ShowSidebar"] = true;

            // Find the prescription by ID
            var prescription = await _context.Prescriptions
                .Include(p => p.Patient)
                .Include(p => p.Doctor)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (prescription == null)
            {
                return NotFound(); // Handle not found case
            }

            // Populate dropdowns with the selected values for Patient and Doctor
            PopulatePatientsDropdown(prescription.PatientId);
            PopulateDoctorsDropdown(prescription.DoctorId);

            return View(prescription);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPrescription(int id, [Bind("Id,PatientId,DoctorId,Medication,Dosage,DateWritten,Status")] Prescription prescription)
        {
            if (id != prescription.Id)
            {
                return NotFound(); // Handle ID mismatch case
            }

           
                try
                {
                    var existingPrescription = await _context.Prescriptions.FindAsync(id);
                    if (existingPrescription == null)
                    {
                        return NotFound();
                    }

                    existingPrescription.PatientId = prescription.PatientId;
                    existingPrescription.DoctorId = prescription.DoctorId;
                    existingPrescription.Medication = prescription.Medication;
                    existingPrescription.Dosage = prescription.Dosage;
                    existingPrescription.DateWritten = prescription.DateWritten;
                    existingPrescription.Status = prescription.Status;

                    _context.Update(existingPrescription);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PrescriptionExists(prescription.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(ListPrescriptions));
            

            // If ModelState is invalid, re-populate dropdowns and return view
            PopulatePatientsDropdown(prescription.PatientId);
            PopulateDoctorsDropdown(prescription.DoctorId);

            return View(prescription);
        }


        // GET: ScriptManager/DeletePrescription
        public async Task<IActionResult> DeletePrescription(int id)
        {
            ViewData["ShowSidebar"] = true;

            // Find the prescription by ID
            var prescription = await _context.Prescriptions
                .Include(p => p.Patient)
                .Include(p => p.Doctor)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (prescription == null)
            {
                return NotFound(); // Handle not found case
            }

            return View(prescription); // Confirm deletion view
        }

        // POST: ScriptManager/DeletePrescription
        [HttpPost, ActionName("DeletePrescription")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ViewData["ShowSidebar"] = true;

            var prescription = await _context.Prescriptions.FindAsync(id);
            if (prescription == null)
            {
                return NotFound(); // Handle not found case
            }

            try
            {
                _context.Prescriptions.Remove(prescription);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                // Optionally handle exceptions, e.g., log the issue or show an error message
                ModelState.AddModelError("", "Unable to delete. Try again, and if the problem persists contact support.");
                return View(prescription);
            }

            return RedirectToAction(nameof(ListPrescriptions));
        }























        private void PopulatePatientsDropdown()
        {
            var patients = _context.Users
                .Where(u => u.Role == UserRole.PATIENT)
                .Select(u => new
                {
                    u.Id,
                    FullName = $"{u.FirstName} {u.LastName}"
                }).ToList();

            ViewBag.Patients = new SelectList(patients, "Id", "FullName");
        }




        private void PopulateMedicationsDropdown(int? selectedMedicationId = null)
        {
            var medications = _context.Medications
                .Where(m => m.DeletionStatus != MedicationStatus.Deleted)
                .ToList();

            ViewBag.MedicationId = new SelectList(medications, "MedicationId", "Name", selectedMedicationId);
        }



        private void PopulateAdmittedPatientsDropdown(int? selectedPatientId = null)
        {
            ViewBag.AdmittedPatients = new SelectList(
                _context.AdmitPatients
                    .Where(ap => ap.AdmitPatientStatus == AdmitPatientStatus.Admitted)
                    .Select(ap => new { ap.Id, FullName = $"{ap.Patient.FirstName} {ap.Patient.LastName}" }),
                "Id", "FullName", selectedPatientId);
        }

        private bool PatientInstructionExists(int id)
        {
            return _context.PatientInstructions.Any(e => e.Id == id);
        }

        private bool PatientDischargeExists(int id)
        {
            return _context.PatientDischargeInstructions.Any(pd => pd.Id == id);
        }

        private bool PrescriptionExists(int id)
        {
            return _context.Prescriptions.Any(p => p.Id == id);
        }

        private void PopulatePatientsDropdown(object selectedPatient = null)
        {
            var patientsQuery = from p in _context.Users
                                orderby p.LastName
                                select new { Id = p.Id, FullName = p.FirstName + " " + p.LastName };

            ViewBag.PatientId = new SelectList(patientsQuery, "Id", "FullName", selectedPatient);
        }

        private void PopulateDoctorsDropdown(object selectedDoctor = null)
        {
            var doctorsQuery = from d in _context.Users
                               orderby d.LastName
                               select new { Id = d.Id, FullName = d.FirstName + " " + d.LastName };

            ViewBag.DoctorId = new SelectList(doctorsQuery, "Id", "FullName", selectedDoctor);
        }



        private bool MedicationPrescriptionExists(int id)
        {
            return _context.MedicationPrescriptions.Any(mp => mp.Id == id);
        }
    }
}

