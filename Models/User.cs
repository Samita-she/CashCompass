using System.Collections.Generic;
namespace CashCompass.API.Models
{
    public class User
    {
        public int UserId { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        
        
        public DateTime CreatedAt { get; set; } 

        public ICollection<Category> Categories { get; set; } = new List<Category>();
        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
        public ICollection<IncomeSource> IncomeSources { get; set; } = new List<IncomeSource>(); 
        public ICollection<Allocation> Allocations { get; set; } = new List<Allocation>(); 
        
    }
}