using System;
using System.ComponentModel.DataAnnotations;

namespace TimelessTechnicians.UI.Models
{
    public class StockLog
    {
        [Key]
        public int StockLogId { get; set; }

        [Required]
        public int ConsumableId { get; set; }

        [Required]
        public int QuantityTaken { get; set; }

        [Required]
        public DateTime DateTaken { get; set; } = DateTime.Now;

        // Navigation property
        public virtual Consumable Consumable { get; set; }
    }
}
