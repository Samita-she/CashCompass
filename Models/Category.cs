using System.ComponentModel.DataAnnotations; 
using System.Collections.Generic;
namespace CashCompass.API.Models
{
    public class Category
    {
        [Key] 
        public int CategoryId { get; set; }
        
        public int UserId { get; set; }

        public string CategoryName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public  User? User { get; set; }

        public ICollection<Allocation> Allocations { get; set; } = new List<Allocation>(); 
        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    }
}