namespace TimelessTechnicians.UI.ViewModel
{
    public class AdmittedPatientsStatisticsViewModel
    {
        public int TotalPatients { get; set; }
        public int ActivePatients { get; set; }
        public int DischargedPatients { get; set; }
        public int PendingPatients { get; set; }
        public List<AdmittedPatientViewModel> AdmittedPatients { get; set; }
    }

}
