using System;
using System.ComponentModel.DataAnnotations;

namespace TimelessTechnicians.UI.Models
{
    public class PatientTreatment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string PatientId { get; set; } // Foreign key to Patient

        [Required]
        [StringLength(500, ErrorMessage = "Treatment description cannot exceed 500 characters.")]
        public string TreatmentDescription { get; set; }

        private DateTime _datePerformed;

        [Required]
        [DataType(DataType.Date)]
        public DateTime DatePerformed
        {
            get => _datePerformed;
            set
            {
                if (value.Date != DateTime.Today)
                {
                    throw new ArgumentException("Date performed must be today's date.");
                }
                _datePerformed = value;
            }
        }

        public PatientTreatment()
        {
            DatePerformed = DateTime.Today; // Default to today's date
        }

        [Required]
        [StringLength(450, ErrorMessage = "Performed by information cannot exceed 450 characters.")]
        public string PerformedBy { get; set; } // Example: Nurse, Doctor, etc.

        [Required]
        public TreatmentStatus TreatmentStatus { get; set; } = TreatmentStatus.Active;

        // Navigation property
        [Required]
        public ApplicationUser Patient { get; set; } // Foreign key relationship to Patient entity
    }

    public enum TreatmentStatus
    {
        Active,
        Deleted
    }
}
