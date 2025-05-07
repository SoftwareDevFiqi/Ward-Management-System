using System;
using System.ComponentModel.DataAnnotations;

namespace TimelessTechnicians.UI.Models
{
    public class ReAdmissionHistory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AdmitPatientId { get; set; } // Foreign key to AdmitPatient entity

        [Required]
        public AdmitPatient AdmitPatient { get; set; } // Navigation property to AdmitPatient

        private DateTime _reAdmissionDate;

        [Required]
        [DataType(DataType.Date)]
        public DateTime ReAdmissionDate
        {
            get => _reAdmissionDate;
            set
            {
                if (value.Date < DateTime.Today)
                {
                    throw new ArgumentException("Re-admission date must be today or a future date.");
                }
                _reAdmissionDate = value;
            }
        }

        [Required]
        [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters.")]
        public string Reason { get; set; } // Reason for the re-admission
    }
}
