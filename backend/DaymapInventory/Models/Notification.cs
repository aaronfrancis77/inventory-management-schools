using System.ComponentModel.DataAnnotations;

namespace DaymapInventory.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Type { get; set; } = NotificationType.LowStock.ToString();

        [Required]
        [MaxLength(500)]
        public string Message { get; set; } = string.Empty;

        public bool IsRead { get; set; } = false;

        [MaxLength(200)]
        public string? RecipientUserId { get; set; }

        public int? ItemId { get; set; }

        public int? ItemInstanceId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [System.ComponentModel.DataAnnotations.Schema.ForeignKey("ItemId")]
        public Item? Item { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.ForeignKey("ItemInstanceId")]
        public ItemInstance? ItemInstance { get; set; }
    }

    public enum NotificationType
    {
        LowStock,
        OutOfStock,
        ExpiryUpcoming,
        ExpiryPassed
    }
}
