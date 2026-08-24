namespace DaymapInventory.Models
{
    public class CreateTransactionDto
    {
        public Guid ItemId { get; set; }
        public Guid? ItemInstanceId { get; set; }
        public Guid LoanedToId { get; set; }
        public int QuantityChanged { get; set; }
        public string Type { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }

    public class TransactionResponseDto
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