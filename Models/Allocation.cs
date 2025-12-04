using System.ComponentModel.DataAnnotations; 
using System;

namespace CashCompass.API.Models
{
    public class Allocation
    {
        [Key] 
        public int AllocationId { get; set; }
        public int UserId { get; set; }
        
        public int IncomeId { get; set; }
        public int CategoryId { get; set; }
        public required string AllocationType { get; set; }   
        public decimal AllocationValue { get; set; }
        public DateTime CreatedAt { get; set; }

        public  User? User { get; set; }
        public  IncomeSource? IncomeSource { get; set; } 
        public  Category? Category { get; set; } 
    }
}