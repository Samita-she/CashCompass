namespace CashCompass.API.Models
{
    public class User
    {
        public int UserId { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        
        // ADDED: This fixes the CS1061 errors in UsersController.cs
        public DateTime CreatedAt { get; set; } 
    }
}