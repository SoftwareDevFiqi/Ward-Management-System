using TimelessTechnicians.UI.Models;

namespace TimelessTechnicians.UI.ViewModel
{
    public class ReAdmissionHistoryViewModel
    {
        public string SearchTerm { get; set; }
        public IEnumerable<ReAdmissionHistory> ReAdmissionHistories { get; set; }

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
