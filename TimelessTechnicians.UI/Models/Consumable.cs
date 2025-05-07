using System;
using System.ComponentModel.DataAnnotations;
using TimelessTechnicians.UI.Services;

namespace TimelessTechnicians.UI.Models
{
    public class Consumable
    {
        [Key]
        public int ConsumableId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; }

        [Required]
        public ConsumableType Type { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a non-negative number.")]
        public int Quantity { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Expiry Date")]
        [FutureDate(ErrorMessage = "Expiry Date must be a future date.")]
        public DateTime ExpiryDate { get; set; }

        public ConsumableStatus DeletionStatus { get; set; }  // Default to Active

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; } // Optional field for additional info

        // Timestamp fields
        [DataType(DataType.DateTime)]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [DataType(DataType.DateTime)]
        [Display(Name = "Last Updated Date")]
        public DateTime LastUpdatedDate { get; set; } = DateTime.Now;

        // Methods to update timestamps
        public void UpdateLastUpdated()
        {
            LastUpdatedDate = DateTime.Now;
        }
    }

    public enum ConsumableType
    {
        Medication,
        Surgical,
        Diagnostic,
        Other
    }

    public enum ConsumableStatus
    {
        Active,
        Approved,
        Rejected, 
        Deleted
    }


}
