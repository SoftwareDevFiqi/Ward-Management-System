using Microsoft.AspNetCore.Mvc.Rendering;

namespace TimelessTechnicians.UI.ViewModel
{
    public class ReAdmitPatientViewModel
    {
        public int AdmitPatientId { get; set; } // ID of the previously discharged patient
        public List<SelectListItem> DischargedPatients { get; set; } // List of discharged patients for dropdown

        public string ReAdmissionReason { get; set; }
    }

}
