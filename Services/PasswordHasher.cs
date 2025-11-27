namespace CashCompass.API.Services
{
    // 1. Interface Definition (Tells us WHAT methods we need)
    // Note: I am placing the interface here for simplicity, 
    // but a robust application might put it in a core layer.
    public interface IPasswordHasher
    {
        string HashPassword(string password);
        bool VerifyPassword(string hashedPassword, string providedPassword);
    }
    
    // 2. Concrete Implementation (Tells us HOW to do it)
    public class PasswordHasher : IPasswordHasher
    {
        // To use real BCrypt, you must run: dotnet add package BCrypt.Net-Next
        // For now, let's use a very simple placeholder for demonstration/testing.
        
        public string HashPassword(string password)
        {
            // WARNING: This is NOT secure for production. 
            // Replace with BCrypt.Net-Next logic later.
            return $"HASH_{password}_SALT"; 
        }

        public bool VerifyPassword(string hashedPassword, string providedPassword)
        {
            // WARNING: This is NOT secure for production.
            return hashedPassword == $"HASH_{providedPassword}_SALT"; 
        }
    }
}