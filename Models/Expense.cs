using System.ComponentModel.DataAnnotations; 
namespace CashCompass.API.Models
{
    public class Expense
    {
        [Key]
        public int ExpenseId { get; set; }   // PK
        
        public int CategoryId { get; set; }  // FK → Category
        public int UserId { get; set; }      // FK → User

        public string ExpenseName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; }

        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public  User? User { get; set; }
        public  Category? Category { get; set; }
    
        
        
    }
}