namespace TimelessTechnicians.UI.Models
{
    public class PatientFolder
    {
        public int Id { get; set; }
        public string PatientId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? NurseId { get; set; } // Make NurseId nullable if desired

        // Navigation properties (optional)
        public virtual ApplicationUser Nurse { get; set; }

        public RecordStatus Status { get; set; } = RecordStatus.Active;
    }

    public enum RecordStatus
    {
        Active,
        Deleted
    }

}
