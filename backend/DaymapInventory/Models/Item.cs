using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DaymapInventory.Models
{
    public class Item
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        public int StockCount { get; set; } = 0;

        public int LowStockThreshold { get; set; } = 5;

        [Required]
        public string Status { get; set; } = ItemStatus.Active.ToString();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<ItemInstance> Instances { get; set; } = new List<ItemInstance>();
        public ICollection<ItemCategory> ItemCategories { get; set; } = new List<ItemCategory>();
        public ICollection<ItemTag> ItemTags { get; set; } = new List<ItemTag>();
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }

    public enum ItemStatus
    {
        Active,
        Disabled,
        Archived
    }
}
