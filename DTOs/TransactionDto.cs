namespace CashCompass.API.DTOs
{
    // Returned to the client
    public class TransactionDto
    {
        public int TransactionId { get; set; }
        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public string Type { get; set; } = string.Empty; // "Income" or "Expense"
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
    }

    // Used when creating a new Transaction
    public class TransactionCreateDto
    {
        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public string Type { get; set; } = string.Empty; // "Income" or "Expense"
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    }

    // Used when updating an existing Transaction
    public class TransactionUpdateDto
    {
        public int CategoryId { get; set; }
        public string Type { get; set; } = string.Empty; // "Income" or "Expense"
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
