using System.ComponentModel.DataAnnotations;

namespace TimelessTechnicians.UI.Models
{
    public class Allergy
    {
        [Key]
        public int AllergyId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; }


        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; } // Optional field for additional info

        [Required]
        public AllergyStatus DeletionStatus { get; set; } // Soft delete status
    }


    public enum AllergyStatus
    {
        [Display(Name = "Active")]
        Active,

        [Display(Name = "Deleted")]
        Deleted
    }
}
