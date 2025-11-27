using System.ComponentModel.DataAnnotations; // 👈 ADD THIS USING DIRECTIVE

namespace CashCompass.API.Models
{
    public class Category
    {
        [Key] // 👈 FIX: Explicitly marks this as the Primary Key
        public int CategoryId { get; set; }
        
        public int UserId { get; set; }

        public string CategoryName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}