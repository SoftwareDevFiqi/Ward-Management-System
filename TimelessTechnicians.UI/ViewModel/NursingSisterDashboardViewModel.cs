using TimelessTechnicians.UI.Models;

namespace TimelessTechnicians.UI.ViewModel
{
    public class NursingSisterDashboardViewModel
    {
        public int TotalAdministeredMedications { get; set; }
        public List<MedicationScheduleGroup> MedicationBySchedule { get; set; }
        public List<MedicationMonthGroup> MedicationPerMonth { get; set; }
        public List<MedicationStatusGroup> MedicationStatusCount { get; set; }
        public double AverageDosage { get; set; }
        public int DeletedMedicationsCount { get; set; }
    }

    public class MedicationScheduleGroup
    {
        public MedicationSchedule Schedule { get; set; }
        public int Count { get; set; }
    }

    public class MedicationMonthGroup
    {
        public int Month { get; set; }
        public int Count { get; set; }
    }

    public class MedicationStatusGroup
    {
        public ScheduledMedicationStatus Status { get; set; }
        public int Count { get; set; }
    }

}
