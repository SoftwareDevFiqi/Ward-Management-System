using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TimelessTechnicians.UI.Services;

namespace TimelessTechnicians.UI.Models
{
    public class BedAssignment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AdmitPatientId { get; set; }

        [Required]
        public int BedId { get; set; }

        [Required]
        [NoPastOrFutureDate(ErrorMessage = "Assigned Date must be today's date.")]
        public DateTime AssignedDate { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual AdmitPatient AdmitPatient { get; set; }
        public virtual Bed Bed { get; set; }

        public BedAssignmentStatus BedAssignmentStatus { get; set; }
    }

    public enum BedAssignmentStatus
    {
        Active,
        Deleted
    }
}
