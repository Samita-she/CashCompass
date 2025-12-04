using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CashCompass.API.Data;
using CashCompass.API.Models;   // ⚡ Models for database
using CashCompass.API.DTOs;     // ⚡ DTOs for API communication

namespace CashCompass.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }

        // GET ALL CATEGORIES
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.Categories
                .Select(c => new CategoryDto
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    Description = c.Description,
                    UserId = c.UserId
                })
                .ToListAsync();

            return Ok(categories);
        }

        // GET CATEGORIES BY USER
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetCategoriesByUser(int userId)
        {
            var categories = await _context.Categories
                .Where(c => c.UserId == userId)
                .Select(c => new CategoryDto
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    UserId = c.UserId,
                    Description = c.Description
                })
                .ToListAsync();

            return Ok(categories);
        }

        // GET CATEGORY BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var category = await _context.Categories
                .Where(c => c.CategoryId == id)
                .Select(c => new CategoryDto
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    UserId = c.UserId,
                    Description = c.Description
                })
                .FirstOrDefaultAsync();

            if (category == null) return NotFound();
            return Ok(category);
        }

        // CREATE CATEGORY
        [HttpPost]
        public async Task<IActionResult> CreateCategory(CategoryCreateDto dto)
        {
            var category = new Category
            {
                CategoryName = dto.CategoryName,
                UserId = dto.UserId,
                Description = dto.Description ?? string.Empty
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            var result = new CategoryDto
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                UserId = category.UserId,
                Description = category.Description
            };

            return CreatedAtAction(nameof(GetCategory), new { id = category.CategoryId }, result);
        }

        // UPDATE CATEGORY
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, CategoryUpdateDto dto)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            category.CategoryName = dto.CategoryName;
            category.Description = dto.Description ?? category.Description;

            await _context.SaveChangesAsync();

            var result = new CategoryDto
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                UserId = category.UserId,
                Description = category.Description
            };

            return Ok(result);
        }

        // DELETE CATEGORY
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
