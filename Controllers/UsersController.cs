using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CashCompass.API.Data;
using CashCompass.API.Models;
using CashCompass.API.DTOs;
using CashCompass.API.Services;


namespace CashCompass.API.Controllers
{
   
    
    
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher _passwordHasher; 

        // Constructor now accepts the IPasswordHasher dependency
        public UsersController(AppDbContext context, IPasswordHasher passwordHasher) 
        {
            _context = context;
            _passwordHasher = passwordHasher; 
        }

        // 📌 GET ALL USERS
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users
                .Select(u => new UserDto
                {
                    UserId = u.UserId,
                    FullName = u.FullName,
                    Email = u.Email,

                    CreatedAt = u.CreatedAt 
                })
                .ToListAsync();

            return Ok(users);
        }

        // 📌 GET USER BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _context.Users
                .Where(u => u.UserId == id)
                .Select(u => new UserDto
                {
                    UserId = u.UserId,
                    FullName = u.FullName,
                    Email = u.Email,
                    CreatedAt = u.CreatedAt 
                })
                .FirstOrDefaultAsync();

            if (user == null) return NotFound();
            return Ok(user);
        }

        // 📌 CREATE USER
        [HttpPost]
        public async Task<IActionResult> CreateUser(UserCreateDto dto)
        {
            // 1. Hash the plain text password from the DTO
            string hashedPassword = _passwordHasher.HashPassword(dto.Password); 
            
            var newUser = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = hashedPassword, 
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            var result = new UserDto
            {
                UserId = newUser.UserId,
                FullName = newUser.FullName,
                Email = newUser.Email,
                
            };

            return CreatedAtAction(nameof(GetUser), new { id = newUser.UserId }, result);
        }

        // 📌 UPDATE USER
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserUpdateDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.FullName = dto.FullName;
            user.Email = dto.Email;

            await _context.SaveChangesAsync();

            var result = new UserDto
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                CreatedAt = user.CreatedAt 
            };

            return Ok(result);
        }

        // 📌 DELETE USER
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}