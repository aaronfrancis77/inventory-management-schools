using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DaymapInventory.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ItemId { get; set; }

        public int? ItemInstanceId { get; set; } // nullable - not all transactions are per-instance

        [Required]
        public string Type { get; set; } = string.Empty; // e.g. Loan, Return, Restock, Adjustment

        public int QuantityChanged { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        [Required]
        [MaxLength(200)]
        public string PerformedBy { get; set; } = string.Empty; // Daymap user identifier (string, not FK)

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties — ignored in JSON to prevent circular references
        [JsonIgnore]
        [ForeignKey("ItemId")]
        public Item? Item { get; set; }

        [JsonIgnore]
        [ForeignKey("ItemInstanceId")]
        public ItemInstance? ItemInstance { get; set; }
    }

    public enum TransactionType
    {
        Loan,
        Return,
        Purchase,
        Restock,
        Adjustment
    }
}
