namespace TimelessTechnicians.UI.ViewModel
{
    public class ScheduledMedicationListViewModel
    {
        public int ScheduledMedicationId { get; set; }
        public int MedicationId { get; set; } // Store selected medication
        public string? MedicationName { get; set; }
        public string Dosage { get; set; }
        public DateTime AdministeredDate { get; set; }
        public string AdministeredBy { get; set; }
        public string ScheduledMedicationStatus { get; set; }
    }
}
