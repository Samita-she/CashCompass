namespace CashCompass.API.DTOs
{
    
   public class UserDto
{
    public int UserId { get; set; }
    public string FullName { get; set; } = null!; 
    public string Email { get; set; } = null!;    
    public DateTime CreatedAt { get; set; }
}

public class UserCreateDto
{
    public string FullName { get; set; } = null!; 
    public string Email { get; set; } = null!;    
    public string Password { get; set; } = null!; 
}

public class UserUpdateDto
{
    public string FullName { get; set; } = null!; 
    public string Email { get; set; } = null!;   
}
}