using TimelessTechnicians.UI.Models;

namespace TimelessTechnicians.UI.ViewModel
{
    public class DoctorAdviceRequestListViewModel
    {
        public int Id { get; set; }
        public string NurseName { get; set; }
        public string DoctorName { get; set; }
        public string PatientName { get; set; }
        public string RequestDetails { get; set; }
        public DateTime RequestDate { get; set; }
        public DoctorAdviceRequestStatus Status { get; set; }
    }

}
