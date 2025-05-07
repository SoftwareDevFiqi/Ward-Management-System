using System;
using System.ComponentModel.DataAnnotations;
using TimelessTechnicians.UI.Services;

namespace TimelessTechnicians.UI.Models
{
    public class Medication
    {
        [Key]
        public int MedicationId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; }

        [Required]
        public MedicationType Type { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a non-negative number.")]
        public int Quantity { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Expiry Date")]
        [FutureDate(ErrorMessage = "Expiry Date must be in the future.")]
        public DateTime ExpiryDate { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; } // Optional field for additional info

        [Required]
        public MedicationSchedule Schedule { get; set; }

        [Required]
        public MedicationStatus DeletionStatus { get; set; } // Soft delete status
    }

    public enum MedicationType
    {
        [Display(Name = "Prescription Medication")]
        Prescription,

        [Display(Name = "Over the Counter")]
        OverTheCounter,

        [Display(Name = "Supplement")]
        Supplement,

        [Display(Name = "Other")]
        Other
    }

    public enum MedicationStatus
    {
        [Display(Name = "Active")]
        Active,

        [Display(Name = "Deleted")]
        Deleted
    }

    public enum MedicationSchedule
    {
        [Display(Name = "Schedule 1")]
        Schedule1,

        [Display(Name = "Schedule 2")]
        Schedule2,

        [Display(Name = "Schedule 3")]
        Schedule3,

        [Display(Name = "Schedule 4")]
        Schedule4,

        [Display(Name = "Schedule 5")]
        Schedule5,

        [Display(Name = "Schedule 6")]
        Schedule6,

        [Display(Name = "Schedule 7")]
        Schedule7
    }
}
