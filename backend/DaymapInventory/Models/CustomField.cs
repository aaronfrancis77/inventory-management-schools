using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DaymapInventory.Models
{
    public class CustomField
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public string DataType { get; set; } = CustomFieldDataType.Text.ToString();

        public bool IsRequired { get; set; } = false;

        public string? DefaultValue { get; set; }

        public int? CategoryId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [JsonIgnore]
        [System.ComponentModel.DataAnnotations.Schema.ForeignKey("CategoryId")]
        public Category? Category { get; set; }
    }

    public enum CustomFieldDataType
    {
        Text,
        Number,
        Date,
        Dropdown,
        Boolean
    }
}