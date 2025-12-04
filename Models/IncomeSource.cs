using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace CashCompass.API.Models
{
    [Table("IncomeSource")]  
    public class IncomeSource
    {
        [Key] 
        public int IncomeId { get; set; } 
        
        public int UserId { get; set; }

        public string SourceName { get; set; } = string.Empty;
        public decimal Amount { get; set; }

        public string PayFrequency { get; set; } = string.Empty;
        public DateTime NextPayDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public  User? User { get; set; }
        public ICollection<Allocation> Allocations { get; set; } = new List<Allocation>();
        
        
    }
}