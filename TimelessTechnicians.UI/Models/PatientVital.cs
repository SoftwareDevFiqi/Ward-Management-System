using System;
using System.ComponentModel.DataAnnotations;

namespace TimelessTechnicians.UI.Models
{
    public class PatientVital
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string PatientId { get; set; }



        public virtual ApplicationUser Patient { get; set; }
       
        [Required]
        [Display(Name = "Blood Pressure")]
        public string BloodPressure { get; set; }

        [Required]
        [Display(Name = "Temperature")]
        public string Temperature { get; set; }

        [Required]
        [Display(Name = "Sugar Level")]
        public string SugarLevel { get; set; }

        private DateTime _recordedDate;

        [Required]
        public DateTime RecordedDate
        {
            get => _recordedDate;
            set
            {
                if (value.Date != DateTime.Today)
                {
                    throw new ArgumentException("Recorded date must be today.");
                }
                _recordedDate = value;
            }
        }

        public PatientVitalStatus PatientVitalStatus { get; set; } = PatientVitalStatus.Active;
        public string? AdministeredBy { get; set; }
    }

    public enum PatientVitalStatus
    {
        Active,
        Deleted
    }

}
