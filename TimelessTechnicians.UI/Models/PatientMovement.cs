using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimelessTechnicians.UI.Models
{
    public class PatientMovement
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(AdmitPatient))]
        public int AdmitPatientId { get; set; }

        [Required]
        [TodayDate(ErrorMessage = "Movement date must be today.")]
        public DateTime MovementDate { get; set; } = DateTime.Now;

        [Required]
        [StringLength(100)]
        public string Location { get; set; }

        [Required]
        public MovementType MovementType { get; set; } = MovementType.CheckIn;

        [StringLength(500)]
        public string? Notes { get; set; } // Optional notes about the movement

        // Navigation property
        public virtual AdmitPatient AdmitPatient { get; set; }

        public MovementStatus MovementStatus { get; set; } = MovementStatus.Active; // Default to Active
    }

    public enum MovementType
    {
        CheckIn,
        CheckOut,
        Transfer,
        Emergency
    }

    public enum MovementStatus
    {
        Active,
        Deleted
    }

    
}
