using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimelessTechnicians.UI.Models
{
    public class Bed
    {
        [Key]
        public int BedId { get; set; }

        [Required]
        [StringLength(50)]
        public string BedNumber { get; set; }

        [Required]
        public BedStatus Status { get; set; }

        [Required]
        public int WardId { get; set; }

        // Navigation property
        [ForeignKey(nameof(WardId))]
        public virtual Ward Ward { get; set; }

        public CondtionStatus DeletionStatus { get; set; }  // Soft delete status
    }

    public enum BedStatus
    {
        [Display(Name = "Available")]
        Available,

        [Display(Name = "Occupied")]
        Occupied,

        [Display(Name = "Under Maintenance")]
        UnderMaintenance
    }

    public enum CondtionStatus
    {
        Active,
        Deleted
    }


}
