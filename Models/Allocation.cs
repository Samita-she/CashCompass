namespace CashCompass.API.Models
{
    public class Allocation
    {
        public int AllocationId { get; set; }
        public int IncomeId { get; set; }
        public int CategoryId { get; set; }
        public required string AllocationType { get; set; }   
        public decimal AllocationValue { get; set; }
        public DateTime CreatedAt { get; set; } // Add this line
    }
}
