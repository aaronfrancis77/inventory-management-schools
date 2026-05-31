using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DaymapInventory.Models
{
    public class CustomFieldValue
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CustomFieldId { get; set; }

        public int? ItemId { get; set; }

        public int? ItemInstanceId { get; set; }

        [Required]
        public string Value { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        [ForeignKey(nameof(CustomFieldId))]
        public CustomField? CustomField { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(ItemId))]
        public Item? Item { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(ItemInstanceId))]
        public ItemInstance? ItemInstance { get; set; }
    }
}
