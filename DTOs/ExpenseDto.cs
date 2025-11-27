namespace CashCompass.API.DTOs
{
    public class ExpenseDto
    {
        public int ExpenseId { get; set; }
        public string ExpenseName { get; set; } = string.Empty; // initialized to avoid warnings
        public decimal Amount { get; set; }
        public int CategoryId { get; set; }
        public DateTime ExpenseDate { get; set; }
        public string? Notes { get; set; }                      // optional
        public DateTime CreatedAt { get; set; }
    }

    public class ExpenseCreateDto
    {
        public string ExpenseName { get; set; } = string.Empty; // initialized
        public decimal Amount { get; set; }
        public int CategoryId { get; set; }
        public DateTime ExpenseDate { get; set; }
        public string? Notes { get; set; }                      // optional
    }

    public class ExpenseUpdateDto
    {
        public string ExpenseName { get; set; } = string.Empty; // initialized
        public decimal Amount { get; set; }
        public int CategoryId { get; set; }
        public DateTime ExpenseDate { get; set; }
        public string? Notes { get; set; }                      // optional
    }
}
