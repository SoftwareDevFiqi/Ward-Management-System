using TimelessTechnicians.UI.Models;

namespace TimelessTechnicians.UI.ViewModel
{
    public class NonScheduledMedicationViewModel
    {
        public int Id { get; set; }
        public int MedicationId { get; set; } // MedicationId to store the selected medication
        public string? MedicationName { get; set; }
        public string Dosage { get; set; }
        public DateTime AdministeredDate { get; set; } = DateTime.Today;
        public string? AdministeredBy { get; set; }

        public NonScheduledMedicationStatus Status { get; set; } = NonScheduledMedicationStatus.Active;
    }



}
