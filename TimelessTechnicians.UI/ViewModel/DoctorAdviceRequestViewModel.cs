using Microsoft.AspNetCore.Mvc.Rendering;

namespace TimelessTechnicians.UI.ViewModel
{
    public class DoctorAdviceRequestViewModel
    {

        public int Id {get;set;}
        public string? NurseId { get; set; } // Nullable NurseId

        public string DoctorId { get; set; }
        public string PatientId { get; set; }
        public string RequestDetails { get; set; }
        public DateTime RequestDate { get; set; }

        // Nullable dropdown lists
        public SelectList? DoctorList { get; set; }
        public SelectList? PatientList { get; set; }
    }


}
