using Microsoft.AspNetCore.Mvc.Rendering;
using TimelessTechnicians.UI.Models;

namespace TimelessTechnicians.UI.ViewModel
{
    public class PatientTreatmentViewModel
    {
        public string PatientId { get; set; }

        public string TreatmentDescription { get; set; }

        public DateTime DatePerformed { get; set; }

        public string PerformedBy { get; set; }


        public TreatmentStatus TreatmentStatus { get; set; } = TreatmentStatus.Active;
        // Add a property to hold the list of patients
        public SelectList? PatientList { get; set; }
    }


}
