using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimelessTechnicians.UI.Models
{
    public class Ward
    {
        [Key]
        public int WardId { get; set; }

        [Required]
        [StringLength(100)]
        public string WardName { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        public WardStatus WardStatus { get; set; }  // Soft delete status
        [Required]
        public int Capacity { get; set; }  // Maximum number of beds allowed

        // Not mapped to database
        [NotMapped]
        public int RemainingCapacity { get; set; }  // Remaining capacity for display




        // Navigation property for related beds
        public virtual ICollection<Bed> Beds { get; set; } = new List<Bed>();  // Initialize to avoid null reference issues
    }

    public enum WardStatus
    {
        Active,
        Deleted
    }
}
