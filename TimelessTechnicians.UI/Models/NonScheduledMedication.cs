using System;
using System.ComponentModel.DataAnnotations;

namespace TimelessTechnicians.UI.Models
{
    public class NonScheduledMedication
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MedicationId { get; set; } // Foreign key to Medication

        [Required]
        [StringLength(100, ErrorMessage = "Dosage cannot exceed 100 characters.")]
        public string Dosage { get; set; }

        [Required]
        [TodayDate(ErrorMessage = "Administered date must be today's date.")]
        public DateTime AdministeredDate { get; set; } = DateTime.Today; // Default to today's date

        [Required]
        [StringLength(450, ErrorMessage = "Administered By cannot exceed 450 characters.")]
        public string AdministeredBy { get; set; } // Assuming it's a User ID or a similar string identifier

        [Required]
        public NonScheduledMedicationStatus Status { get; set; } = NonScheduledMedicationStatus.Active;

        // Navigation Properties
        [Required]
        public virtual Medication Medication { get; set; } // Foreign key relationship to Medication
    }

    public enum NonScheduledMedicationStatus
    {
        Active,
        Deleted
    }

    // Custom attribute to ensure the date is today's date
    public class TodayDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime date)
            {
                if (date.Date != DateTime.Today)
                {
                    return new ValidationResult(ErrorMessage ?? "The date must be today's date.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
