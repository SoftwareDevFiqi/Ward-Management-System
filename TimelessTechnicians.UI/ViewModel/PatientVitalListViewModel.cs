namespace TimelessTechnicians.UI.ViewModel
{
    public class PatientVitalListViewModel
    {
        public int Id { get; set; }
        public string PatientFullName { get; set; }
        public string BloodPressure { get; set; }
        public string Temperature { get; set; }
        public string SugarLevel { get; set; }
        public DateTime RecordedDate { get; set; }
    }

}
