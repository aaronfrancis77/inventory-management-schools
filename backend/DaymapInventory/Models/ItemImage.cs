using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DaymapInventory.Models
{
    public class ItemImage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ItemId { get; set; }

        [Required]
        [MaxLength(500)]
        public string ImageUrl { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? AltText { get; set; }

        public bool IsPrimary { get; set; } = false;

        public int DisplayOrder { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [JsonIgnore]
        [System.ComponentModel.DataAnnotations.Schema.ForeignKey("ItemId")]
        public Item? Item { get; set; }
    }
}
