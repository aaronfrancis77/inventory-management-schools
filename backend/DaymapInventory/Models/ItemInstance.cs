using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DaymapInventory.Models
{
    public class ItemInstance
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ItemId { get; set; }

        [MaxLength(100)]
        public string? SerialNumber { get; set; }

        public DateTime? ExpiryDate { get; set; }

        [Required]
        public string Status { get; set; } = InstanceStatus.Available.ToString();

        public string? Metadata { get; set; } // JSON string for per-unit data

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties — ignored in JSON to prevent circular references
        [JsonIgnore]
        [ForeignKey("ItemId")]
        public Item? Item { get; set; }
    }

    public enum InstanceStatus
    {
        Available,
        Loaned,
        Expired,
        Retired
    }
}
