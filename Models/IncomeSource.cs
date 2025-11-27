using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CashCompass.API.Models
{
    [Table("IncomeSource")]  
    public class IncomeSource
    {
        [Key] // 👈 THIS IS THE CRUCIAL, MISSING FIX
        public int IncomeId { get; set; } 
        
        public int UserId { get; set; }

        public string SourceName { get; set; } = string.Empty;
        public decimal Amount { get; set; }

        public string PayFrequency { get; set; } = string.Empty;
        public DateTime NextPayDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation property (often required for EF Core relationships)
        // public User User { get; set; } = null!; 
    }
}