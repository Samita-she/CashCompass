namespace CashCompass.API.DTOs
{
    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty; 
        public string? Description { get; set; }               
        public int UserId { get; set; }
    }

    public class CategoryCreateDto
    {
        public string CategoryName { get; set; } = string.Empty;
        public string? Description { get; set; }             
        public int UserId { get; set; }
    }

    public class CategoryUpdateDto
    {
        public string CategoryName { get; set; } = string.Empty;
        public string? Description { get; set; }  
        public int UserId { get; set; }           
    }
}
