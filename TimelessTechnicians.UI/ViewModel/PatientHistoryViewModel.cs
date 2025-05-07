using TimelessTechnicians.UI.Models;

namespace TimelessTechnicians.UI.ViewModel
{
    // Inside PatientHistoryViewModel.cs
    public class PatientHistoryViewModel
    {
        public string FullName { get; set; }
        public List<AdmissionHistory> Admissions { get; set; }
        public List<PatientAllergy> Allergies { get; set; }
        public List<PatientMedication> Medications { get; set; }
        public List<PatientCondition> Conditions { get; set; }
        public List<PatientTreatment> Treatments { get; set; }
        public List<PatientVital> Vitals { get; set; }
    }


    public class AdmissionHistory
    {
        public DateTime AdmissionDate { get; set; }
        public AdmitPatientStatus Status { get; set; }
    }

}
