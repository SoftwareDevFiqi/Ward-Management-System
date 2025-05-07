using System;
using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace TimelessTechnicians.UI.Models
{
    public class PatientAppointment
    {
        public int Id { get; set; }

        // Foreign key to AdmitPatient
        public int AdmitPatientId { get; set; }
        public AdmitPatient AdmitPatient { get; set; } // Navigation property

        // Foreign key to Doctor
        [Required]
        public string DoctorId { get; set; }
        public ApplicationUser Doctor { get; set; } // Navigation property to Doctor

        // Appointment-specific details
        [Required]
        [FutureDate(ErrorMessage = "Appointment date must be in the future.")]
        public DateTime AppointmentDate { get; set; }

        [Required]
        [StringLength(500)]
        public string Reason { get; set; }

        public AppointmentStatus Status { get; set; } // Enum to track status (Scheduled, Cancelled, etc.)
    }

    // Supporting Enum for Appointment Status
    public enum AppointmentStatus
    {
        Scheduled,
        Completed,
        Cancelled
    }



    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var date = (DateTime)value;

            if (date <= DateTime.Now)
            {
                return new ValidationResult(ErrorMessage ?? "Date must be in the future.");
            }

            return ValidationResult.Success;
        }
    }
}
