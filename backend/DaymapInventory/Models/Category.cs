using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DaymapInventory.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties — ignored in JSON to prevent circular references
        [JsonIgnore] public ICollection<ItemCategory> ItemCategories { get; set; } = new List<ItemCategory>();
        [JsonIgnore] public ICollection<CategoryTag> CategoryTags { get; set; } = new List<CategoryTag>();
    }
}
