using System.ComponentModel.DataAnnotations; // 👈 ADD THIS USING DIRECTIVE

namespace CashCompass.API.Models
{
    public class Expense
    {
        [Key] // 👈 FIX: Explicitly marks this as the Primary Key
        public int ExpenseId { get; set; }   // PK
        
        public int CategoryId { get; set; }  // FK → Category
        public int UserId { get; set; }      // FK → User

        public string ExpenseName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; }

        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // You should also define the navigation properties later, e.g.:
        // public User User { get; set; } = null!;
        // public Category Category { get; set; } = null!;
    }
}