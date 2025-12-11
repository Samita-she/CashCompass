using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CashCompass.API.Data;
using CashCompass.API.Models;   
using CashCompass.API.DTOs;    

namespace CashCompass.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExpensesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ExpensesController(AppDbContext context)
        {
            _context = context;
        }

        // ⭐ NEW HELPER: Converts any incoming DateTime to UTC kind,
        // which is required by PostgreSQL for 'timestamp with time zone' fields.
        private DateTime ConvertToUtc(DateTime date)
        {
            // If the Kind is Unspecified (common for JSON from Flutter), 
            // treat it as UTC. Otherwise, convert it.
            if (date.Kind == DateTimeKind.Unspecified || date.Kind == DateTimeKind.Local)
            {
                return DateTime.SpecifyKind(date, DateTimeKind.Utc);
            }
            return date.ToUniversalTime();
        }

        // Helper method to create a clean ExpenseDto projection
        private static ExpenseDto ToDto(Expense e)
        {
            return new ExpenseDto
            {
                ExpenseId = e.ExpenseId,
                ExpenseName = e.ExpenseName,
                Amount = e.Amount,
                CategoryId = e.CategoryId,
                // ⭐ FIX: Include UserId in the DTO projection
                UserId = e.UserId, 
                ExpenseDate = e.ExpenseDate,
                CreatedAt = e.CreatedAt, // Database should store this in UTC
                Notes = e.Notes
            };
        }

        // GET ALL EXPENSES
        [HttpGet]
        public async Task<IActionResult> GetExpenses()
        {
            var expenses = await _context.Expenses
                .Select(e => ToDto(e)) // Using the helper DTO projection
                .ToListAsync();

            return Ok(expenses);
        }

        // GET EXPENSES BY CATEGORY
        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetByCategory(int categoryId)
        {
            var expenses = await _context.Expenses
                .Where(e => e.CategoryId == categoryId)
                .Select(e => ToDto(e)) // Using the helper DTO projection
                .ToListAsync();

            return Ok(expenses);
        }

        // GET EXPENSE BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetExpense(int id)
        {
            var e = await _context.Expenses.FindAsync(id);
            if (e == null) return NotFound();

            var dto = ToDto(e); // Using the helper DTO projection

            return Ok(dto);
        }

        // CREATE EXPENSE
        [HttpPost]
        public async Task<IActionResult> CreateExpense(ExpenseCreateDto dto)
        {
            var expense = new Expense
            {
                ExpenseName = dto.ExpenseName,
                Amount = dto.Amount,
                CategoryId = dto.CategoryId,
                UserId = dto.UserId,
                // ⭐ FIX APPLIED: Ensure date is stored as UTC
                ExpenseDate = ConvertToUtc(dto.ExpenseDate), 
                Notes = dto.Notes,
                // CreatedAt will be set automatically, but if model uses it, 
                // it should also be UTC (DateTime.UtcNow handles this implicitly).
            };

            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();

            var result = ToDto(expense); // Using the helper DTO projection

            return CreatedAtAction(nameof(GetExpense), new { id = expense.ExpenseId }, result);
        }

        // UPDATE EXPENSE
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExpense(int id, ExpenseUpdateDto dto)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null) return NotFound();

            // ⭐ CRITICAL FIX: Ensure all fields are mapped
            expense.ExpenseName = dto.ExpenseName;
            expense.Amount = dto.Amount;
            expense.CategoryId = dto.CategoryId;
            
            // ⭐ CRITICAL FIX: Add mapping for the UserId field
            expense.UserId = dto.UserId;
            
            // ⭐ FIX APPLIED: Ensure date is stored as UTC
            expense.ExpenseDate = ConvertToUtc(dto.ExpenseDate); 
            expense.Notes = dto.Notes;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Expenses.Any(e => e.ExpenseId == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            var result = ToDto(expense); // Using the helper DTO projection

            return Ok(result);
        }

        // DELETE EXPENSE
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpense(int id)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null) return NotFound();

            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}