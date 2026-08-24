using System.ComponentModel.DataAnnotations;

namespace DaymapInventory.Models
{
    public class CreateTransactionDtos
    {
        [Required]
        public Guid ItemId { get; set; }

        public Guid? ItemInstanceId { get; set; }

        [Required]
        public Guid LoanedToId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "QuantityChanged must be at least 1.")]
        public int QuantityChanged { get; set; }

        [Required]
        [RegularExpression("Loan|Return", ErrorMessage = "Type must be either 'Loan' or 'Return'.")]
        public string Type { get; set; } = string.Empty;

        public string? Notes { get; set; }
    }

    public class TransactionResponseDtos
    {
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }
        public Guid? ItemInstanceId { get; set; }
        public Guid LoanedToId { get; set; }
        public int QuantityChanged { get; set; }
        public string Type { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public string Status { get; set; } = "Completed";
        public DateTime CreatedAt { get; set; }
    }
}