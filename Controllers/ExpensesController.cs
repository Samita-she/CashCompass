using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CashCompass.API.Data;
using CashCompass.API.Models;   // ⚡ Models for database
using CashCompass.API.DTOs;     // ⚡ DTOs for API communication

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

        // GET ALL EXPENSES
        [HttpGet]
        public async Task<IActionResult> GetExpenses()
        {
            var expenses = await _context.Expenses
                .Select(e => new ExpenseDto
                {
                    ExpenseId = e.ExpenseId,
                    ExpenseName = e.ExpenseName,
                    Amount = e.Amount,
                    CategoryId = e.CategoryId,
                    ExpenseDate = e.ExpenseDate,
                    Notes = e.Notes,
                    CreatedAt = e.CreatedAt
                })
                .ToListAsync();

            return Ok(expenses);
        }

        // GET EXPENSES BY CATEGORY
        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetByCategory(int categoryId)
        {
            var expenses = await _context.Expenses
                .Where(e => e.CategoryId == categoryId)
                .Select(e => new ExpenseDto
                {
                    ExpenseId = e.ExpenseId,
                    ExpenseName = e.ExpenseName,
                    Amount = e.Amount,
                    CategoryId = e.CategoryId,
                    ExpenseDate = e.ExpenseDate,
                    Notes = e.Notes,
                    CreatedAt = e.CreatedAt
                })
                .ToListAsync();

            return Ok(expenses);
        }

        // GET EXPENSE BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetExpense(int id)
        {
            var e = await _context.Expenses.FindAsync(id);
            if (e == null) return NotFound();

            var dto = new ExpenseDto
            {
                ExpenseId = e.ExpenseId,
                ExpenseName = e.ExpenseName,
                Amount = e.Amount,
                CategoryId = e.CategoryId,
                ExpenseDate = e.ExpenseDate,
                Notes = e.Notes,
                CreatedAt = e.CreatedAt
            };

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
                ExpenseDate = dto.ExpenseDate,
                Notes = dto.Notes,
                CreatedAt = DateTime.UtcNow
            };

            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();

            var result = new ExpenseDto
            {
                ExpenseId = expense.ExpenseId,
                ExpenseName = expense.ExpenseName,
                Amount = expense.Amount,
                CategoryId = expense.CategoryId,
                ExpenseDate = expense.ExpenseDate,
                Notes = expense.Notes,
                CreatedAt = expense.CreatedAt
            };

            return CreatedAtAction(nameof(GetExpense), new { id = expense.ExpenseId }, result);
        }

        // UPDATE EXPENSE
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExpense(int id, ExpenseUpdateDto dto)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null) return NotFound();

            expense.ExpenseName = dto.ExpenseName;
            expense.Amount = dto.Amount;
            expense.CategoryId = dto.CategoryId;
            expense.ExpenseDate = dto.ExpenseDate;
            expense.Notes = dto.Notes;

            await _context.SaveChangesAsync();

            var result = new ExpenseDto
            {
                ExpenseId = expense.ExpenseId,
                ExpenseName = expense.ExpenseName,
                Amount = expense.Amount,
                CategoryId = expense.CategoryId,
                ExpenseDate = expense.ExpenseDate,
                Notes = expense.Notes,
                CreatedAt = expense.CreatedAt
            };

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
