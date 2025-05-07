namespace TimelessTechnicians.UI.ViewModel
{
    public class AdminDashboardViewModel
    {
        // Patient Statistics
        public int TotalPatients { get; set; }
        public int PatientsAdmittedToday { get; set; }

        // Bed Availability
        public int TotalBeds { get; set; }
        public int AvailableBeds { get; set; }
        public int OccupiedBeds { get; set; }

        // Allergy Statistics
        public int TotalAllergies { get; set; }
        public int ActiveAllergies { get; set; }

        // Medication Overview
        public int TotalMedications { get; set; }

        // Additional Statistics
        public int TotalWards { get; set; }
        public int TotalConditions { get; set; }
        public int TotalConsumables { get; set; }

        // Active Users
        public int ActiveUsers { get; set; }

        // Employee Statistics
        public int TotalDoctors { get; set; }
        public int TotalWardAdmins { get; set; }
        public int TotalNurses { get; set; }
        public int TotalNursingSisters { get; set; }
        public int TotalScriptManagers { get; set; }
        public int TotalConsumableManagers { get; set; }
    }
}
