using System;
using System.ComponentModel.DataAnnotations;
using TimelessTechnicians.UI.Services;

namespace TimelessTechnicians.UI.Models
{
    public class DoctorAdviceRequest
    {
        [Key]
        public int Id { get; set; }

        [StringLength(450)] // Assuming max length for UserId (for example, GUID length)
        public string? NurseId { get; set; } // Make nullable

        public ApplicationUser? Nurse { get; set; } // Navigation property

        [Required]
        [StringLength(450)] // Assuming max length for UserId (for example, GUID length)
        public string DoctorId { get; set; }

        [Required]
        public ApplicationUser Doctor { get; set; } // Navigation property

        [Required]
        [StringLength(450)] // Assuming max length for UserId (for example, GUID length)
        public string PatientId { get; set; }

        [Required]
        public ApplicationUser Patient { get; set; } // Navigation property

        [Required]
        [StringLength(1000, ErrorMessage = "Request details cannot exceed 1000 characters.")]
        public string RequestDetails { get; set; }

        [Required]
        [TodayDate(ErrorMessage = "Request Date must be today's date.")]
        public DateTime RequestDate { get; set; } = DateTime.Now; // Default to current date

        [Required]
        public DoctorAdviceRequestStatus Status { get; set; } // Enum type for status

  
    }

    public enum DoctorAdviceRequestStatus
    {
        Pending,
        Completed,
        Deleted
    }
}
