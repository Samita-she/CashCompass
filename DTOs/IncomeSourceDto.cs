namespace CashCompass.API.DTOs
{
    // Used when creating a new IncomeSource
    public class IncomeSourceCreateDto
    {
        public string SourceName { get; set; } = string.Empty; // initialized
        public decimal Amount { get; set; }
        public int UserId { get; set; }
    }

    // Used when updating an IncomeSource
    public class IncomeSourceUpdateDto
    {
        public string SourceName { get; set; } = string.Empty; // initialized
        public decimal Amount { get; set; }
    }

    // Returned to the client
    public class IncomeSourceDto
    {
        public int IncomeId { get; set; }
        public string SourceName { get; set; } = string.Empty; // initialized
        public decimal Amount { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
