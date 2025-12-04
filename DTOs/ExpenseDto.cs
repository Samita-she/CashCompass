namespace CashCompass.API.DTOs
{
    public class ExpenseDto
    {
        public int ExpenseId { get; set; }
        public string ExpenseName { get; set; } = string.Empty; 
        public decimal Amount { get; set; }
        public int CategoryId { get; set; }
        public int UserId { get; set; } 
        public DateTime ExpenseDate { get; set; }
        public string? Notes { get; set; }                      
        public DateTime CreatedAt { get; set; }
    }

    public class ExpenseCreateDto
    {
        public string ExpenseName { get; set; } = string.Empty; 
        public decimal Amount { get; set; }
        public int CategoryId { get; set; }
        public int UserId { get; set; } 
        public DateTime ExpenseDate { get; set; }
        public string? Notes { get; set; }                      
    }

    public class ExpenseUpdateDto
    {
        public string ExpenseName { get; set; } = string.Empty; 
        public decimal Amount { get; set; }
        public int CategoryId { get; set; }
        public int UserId { get; set; } 
        public DateTime ExpenseDate { get; set; }
        public string? Notes { get; set; }                      
    }
}