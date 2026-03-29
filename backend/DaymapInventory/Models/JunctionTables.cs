using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DaymapInventory.Models
{
    public class ItemCategory
    {
        public int ItemId { get; set; }
        public int CategoryId { get; set; }

        [ForeignKey("ItemId")]
        public Item? Item { get; set; }

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }
    }

    public class ItemTag
    {
        public int ItemId { get; set; }
        public int TagId { get; set; }

        [ForeignKey("ItemId")]
        public Item? Item { get; set; }

        [ForeignKey("TagId")]
        public Tag? Tag { get; set; }
    }

    public class CategoryTag
    {
        public int CategoryId { get; set; }
        public int TagId { get; set; }

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        [ForeignKey("TagId")]
        public Tag? Tag { get; set; }
    }
}
