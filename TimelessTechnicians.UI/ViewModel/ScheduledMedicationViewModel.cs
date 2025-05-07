using System.ComponentModel.DataAnnotations;
using TimelessTechnicians.UI.Models;

namespace TimelessTechnicians.UI.ViewModel
{
    public class ScheduledMedicationViewModel
    {
        [Key]
        public int ScheduledMedicationId { get; set; }

        [Required]
        public int MedicationId { get; set; }

        [Required]
        public string Dosage { get; set; }

        public DateTime AdministeredDate { get; set; } = DateTime.Now;

        public string? AdministeredBy { get; set; }

        public ScheduledMedicationStatus ScheduledMedicationStatus { get; set; } = ScheduledMedicationStatus.Active;
    }
}
