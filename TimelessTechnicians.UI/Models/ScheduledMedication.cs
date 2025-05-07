using System;
using System.ComponentModel.DataAnnotations;

namespace TimelessTechnicians.UI.Models
{
    public class ScheduledMedication
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MedicationId { get; set; } // Foreign key to Medication

        [Required]
        [StringLength(100, ErrorMessage = "Dosage cannot exceed 100 characters.")]
        public string Dosage { get; set; }

        private DateTime _administeredDate;

        [Required]
        [DataType(DataType.Date)]
        public DateTime AdministeredDate
        {
            get => _administeredDate;
            set
            {
                if (value.Date < DateTime.Today)
                {
                    throw new ArgumentException("Administered date must be today or a future date.");
                }
                _administeredDate = value;
            }
        }

        [StringLength(450, ErrorMessage = "Administered By cannot exceed 450 characters.")]
        public string? AdministeredBy { get; set; }

        // Navigation property
        [Required]
        public virtual Medication Medication { get; set; }

        [Required]
        public ScheduledMedicationStatus ScheduledMedicationStatus { get; set; } = ScheduledMedicationStatus.Active;
    }

    public enum ScheduledMedicationStatus
    {
        Active,
        Completed,
        Cancelled,
        Deleted
    }
}
