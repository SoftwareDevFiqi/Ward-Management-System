using System;
using System.ComponentModel.DataAnnotations;

namespace TimelessTechnicians.UI.Models
{
    public class StockRequest
    {
        [Key]
        public int StockRequestId { get; set; }

        [Required]
        public int ConsumableId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Request quantity must be at least 1.")]
        public int RequestedQuantity { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime RequestDate { get; set; } = DateTime.Now;

        [Required]
        public ConsumableStatus RequestStatus { get; set; } = ConsumableStatus.Active; // Default to Active

        // Navigation property
        public virtual Consumable Consumable { get; set; }
    }
}
