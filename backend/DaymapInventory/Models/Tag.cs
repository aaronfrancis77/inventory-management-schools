using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DaymapInventory.Models
{
    public class Tag
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public bool IsDefault { get; set; } = false; // true = Daymap-provided, false = user-created

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties — ignored in JSON to prevent circular references
        [JsonIgnore] public ICollection<ItemTag> ItemTags { get; set; } = new List<ItemTag>();
        [JsonIgnore] public ICollection<CategoryTag> CategoryTags { get; set; } = new List<CategoryTag>();
    }
}
