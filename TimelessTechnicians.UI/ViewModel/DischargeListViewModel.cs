namespace TimelessTechnicians.UI.ViewModel
{
    public class DischargeListViewModel
    {
        public int Id { get; set; }
        public int AdmitPatientId { get; set; }
        public DateTime DischargeDate { get; set; }
        public string Notes { get; set; }
        public string PatientName { get; set; } // Display the patient's name


        public IEnumerable<DischargeListViewModel> Discharges { get; set; }
        public string SearchTerm { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

}
