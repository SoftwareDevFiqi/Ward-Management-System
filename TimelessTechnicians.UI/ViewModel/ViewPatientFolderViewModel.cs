namespace TimelessTechnicians.UI.ViewModel
{
    public class ViewPatientFolderViewModel
    {
        public string PatientId { get; set; }
        public int PatientFolderId { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<string> Allergies { get; set; }
        public List<string> Medications { get; set; }
        public List<string> Conditions { get; set; }
        public string FullName { get; set; }
        public List<PatientTreatmentListViewModel> Treatments { get; set; }
        public List<PatientVitalListViewModel> Vitals { get; set; }
        public bool IsDoctor { get; set; }
        public string BedNumber { get; set; }
        public string WardName { get; set; }
        public bool IsFolderDeleted { get; set; }
    }

}
