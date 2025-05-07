using System;
using System.ComponentModel.DataAnnotations;
using TimelessTechnicians.UI.Services;

namespace TimelessTechnicians.UI.Models
{
    public class Discharge
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AdmitPatientId { get; set; }

        [Required]
        [TodayDate(ErrorMessage = "Discharge Date must be today's date.")]
        public DateTime DischargeDate { get; set; } = DateTime.Now;

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters.")]
        public string? Notes { get; set; } // Optional notes about discharge

        // Navigation properties
        [Required]
        public virtual AdmitPatient AdmitPatient { get; set; }

        [Required]
        public DischargeStatus DischargeStatus { get; set; }
    }

    public enum DischargeStatus
    {
        Active,
        Deleted
    }
}
