using TimelessTechnicians.UI.ViewModel;

namespace TimelessTechnicians.UI.ViewModels
{
    public class PatientFolderViewModel
    {
        public string PatientId { get; set; }
        public string FullName { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<string> Allergies { get; set; } = new List<string>();
        public List<MedicationViewModel> Medications { get; set; } = new List<MedicationViewModel>();
        public List<ConditionViewModel> Conditions { get; set; } = new List<ConditionViewModel>();
        
    }



}
