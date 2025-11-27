namespace CashCompass.API.DTOs;

public class AllocationDto
{
    public int AllocationId { get; set; }
    public string AllocationName { get; set; } = string.Empty;  // use required or default
    public decimal AllocationValue { get; set; }
    public int UserId { get; set; }
    public int IncomeId { get; set; }         // Add if controller references it
    public int CategoryId { get; set; }       // Add if controller references it
    public string AllocationType { get; set; } = string.Empty; // e.g., "fixed" or "variable"
    public DateTime CreatedAt { get; set; }   // Add if controller references it
}

public class AllocationCreateDto
{
    public string AllocationName { get; set; } = string.Empty;
    public decimal AllocationValue { get; set; }
    public int UserId { get; set; }
    public int IncomeId { get; set; }         
    public int CategoryId { get; set; }       
    public string AllocationType { get; set; } = string.Empty;
}

public class AllocationUpdateDto
{
    public string AllocationName { get; set; } = string.Empty;
    public decimal AllocationValue { get; set; }
    public int CategoryId { get; set; }       // Add if updating allocation’s category
    public string AllocationType { get; set; } = string.Empty;
}
