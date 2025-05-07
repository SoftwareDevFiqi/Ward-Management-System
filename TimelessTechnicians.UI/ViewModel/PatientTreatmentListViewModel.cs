using TimelessTechnicians.UI.Models;

namespace TimelessTechnicians.UI.ViewModel
{
    public class PatientTreatmentListViewModel
    {
        public int Id { get; set; }
        public string PatientFullName { get; set; }
        public string TreatmentDescription { get; set; }
        public DateTime DatePerformed { get; set; }
        public TreatmentStatus TreatmentStatus { get; set; } // Include status in the view model
    }

}
