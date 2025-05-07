using System;
using System.ComponentModel.DataAnnotations;

namespace TimelessTechnicians.UI.Models
{
    public class PatientVisitSchedule
    {
        public int Id { get; set; }

        public int AdmitPatientId { get; set; }
        public AdmitPatient AdmitPatient { get; set; }

        private DateTime _scheduledDate;

        [Required]
        public DateTime ScheduledDate
        {
            get => _scheduledDate;
            set
            {
                if (value.Date < DateTime.Today)
                {
                    throw new ArgumentException("Scheduled date must be today or a future date.");
                }
                _scheduledDate = value;
            }
        }

        [Required]
        [StringLength(500, ErrorMessage = "Visit reason cannot exceed 500 characters.")]
        public string VisitReason { get; set; }

        public VisitStatus Status { get; set; } = VisitStatus.Scheduled; // Enum for visit status

        public DateTime? CompletedDate { get; set; } // Nullable, to track completion date
    }

    public enum VisitStatus
    {
        Scheduled,
        Completed,
        Cancelled
    }
}
