namespace TimelessTechnicians.UI.ViewModel
{
    public class WardAdminDashboardViewModel
    {
        public int TotalPatients { get; set; }
        public int ConfirmedPatients { get; set; }
        public int PendingConfirmations { get; set; }
        public int PatientsWithFolder { get; set; }
        public int TotalDischarges { get; set; }

        // New properties for bed statistics
        public int TotalBeds { get; set; }
        public int OccupiedBeds { get; set; }
        public int AvailableBeds { get; set; }

        public Dictionary<string, int> BedsByWard { get; set; } = new Dictionary<string, int>();
    }
}
