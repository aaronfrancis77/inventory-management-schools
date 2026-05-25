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

        [Required]
        public int ItemId { get; set; }

        [Required]
        public string ControlType { get; set; } = CustomFieldControlType.Text.ToString();

        [Required]
        public string DataType { get; set; } = CustomFieldDataType.String.ToString();

        public bool IsRequired { get; set; } = false;

        public bool IsUnique { get; set; } = false;

        public string? DefaultValue { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        [System.ComponentModel.DataAnnotations.Schema.ForeignKey("ItemId")]
        public Item? Item { get; set; }

        [JsonIgnore]
        public ICollection<CustomFieldValue> Values { get; set; } = new List<CustomFieldValue>();
    }

    public enum CustomFieldControlType
    {
        Label,
        Text,
        Number,
        Date,
        DateTime,
        Time,
        Dropdown,
        Autocomplete,
        Radio,
        Checkbox,
        Slider
    }

    public enum CustomFieldDataType
    {
        String,
        Number,
        Boolean,
        Date,
        DateTime,
        Time,
        Integer
    }
}
