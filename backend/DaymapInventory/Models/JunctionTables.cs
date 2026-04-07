using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DaymapInventory.Models
{
    public class ItemCategory
    {
        public int ItemId { get; set; }
        public int CategoryId { get; set; }

        [JsonIgnore] [ForeignKey("ItemId")] public Item? Item { get; set; }
        [JsonIgnore] [ForeignKey("CategoryId")] public Category? Category { get; set; }
    }

    public class ItemTag
    {
        public int ItemId { get; set; }
        public int TagId { get; set; }

        [JsonIgnore] [ForeignKey("ItemId")] public Item? Item { get; set; }
        [JsonIgnore] [ForeignKey("TagId")] public Tag? Tag { get; set; }
    }

    public class CategoryTag
    {
        public int CategoryId { get; set; }
        public int TagId { get; set; }

        [JsonIgnore] [ForeignKey("CategoryId")] public Category? Category { get; set; }
        [JsonIgnore] [ForeignKey("TagId")] public Tag? Tag { get; set; }
    }
}
