using System.ComponentModel.DataAnnotations; // 👈 ADD THIS USING DIRECTIVE

namespace CashCompass.API.Models
{
    public class Transaction
    {
        [Key] // 👈 FIX: Explicitly marks this as the Primary Key
        public int TransactionId { get; set; }  // PK
        
        public int UserId { get; set; }         // FK → User
        public int CategoryId { get; set; }     // FK → Category

        public string Type { get; set; } = string.Empty; 
        // "Income" or "Expense"

        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}