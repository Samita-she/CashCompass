namespace CashCompass.API.DTOs
{
    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty; // initialize to avoid warnings
        public string? Description { get; set; }               // nullable if optional
        public int UserId { get; set; }
    }

    public class CategoryCreateDto
    {
        public string CategoryName { get; set; } = string.Empty;
        public string? Description { get; set; }              // optional
        public int UserId { get; set; }
    }

    public class CategoryUpdateDto
    {
        public string CategoryName { get; set; } = string.Empty;
        public string? Description { get; set; }             // optional
    }
}
