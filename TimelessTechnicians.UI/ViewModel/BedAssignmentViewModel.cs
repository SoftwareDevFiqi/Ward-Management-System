namespace TimelessTechnicians.UI.ViewModel
{
    public class BedAssignmentViewModel
    {
        public int Id { get; set; }
        public string PatientName { get; set; }
        public string BedNumber { get; set; }
        public string WardName { get; set; }
        public DateTime AssignedDate { get; set; }
    }
}
