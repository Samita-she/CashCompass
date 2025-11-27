using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CashCompass.API.Data;
using CashCompass.API.Models; // ✅ Transaction entity
using CashCompass.API.DTOs;   // ✅ DTOs

namespace CashCompass.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TransactionsController(AppDbContext context)
        {
            _context = context;
        }

        // -------------------------------------------------------------
        // 📌 GET ALL TRANSACTIONS
        // -------------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetTransactions()
        {
            var transactions = await _context.Transactions
                .Select(t => new TransactionDto
                {
                    TransactionId = t.TransactionId,
                    UserId = t.UserId,
                    CategoryId = t.CategoryId,
                    Type = t.Type,
                    Amount = t.Amount,
                    TransactionDate = t.TransactionDate
                })
                .ToListAsync();

            return Ok(transactions);
        }

        // -------------------------------------------------------------
        // 📌 GET TRANSACTIONS BY USER
        // -------------------------------------------------------------
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(int userId)
        {
            var transactions = await _context.Transactions
                .Where(t => t.UserId == userId)
                .Select(t => new TransactionDto
                {
                    TransactionId = t.TransactionId,
                    UserId = t.UserId,
                    CategoryId = t.CategoryId,
                    Type = t.Type,
                    Amount = t.Amount,
                    TransactionDate = t.TransactionDate
                })
                .ToListAsync();

            return Ok(transactions);
        }

        // -------------------------------------------------------------
        // 📌 GET TRANSACTION BY ID
        // -------------------------------------------------------------
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTransaction(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null) return NotFound();

            var dto = new TransactionDto
            {
                TransactionId = transaction.TransactionId,
                UserId = transaction.UserId,
                CategoryId = transaction.CategoryId,
                Type = transaction.Type,
                Amount = transaction.Amount,
                TransactionDate = transaction.TransactionDate
            };

            return Ok(dto);
        }

        // -------------------------------------------------------------
        // 📌 CREATE TRANSACTION
        // -------------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> CreateTransaction(TransactionCreateDto dto)
        {
            var transaction = new Transaction
            {
                UserId = dto.UserId,
                CategoryId = dto.CategoryId,
                Type = dto.Type,
                Amount = dto.Amount,
                TransactionDate = dto.TransactionDate
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            var returnDto = new TransactionDto
            {
                TransactionId = transaction.TransactionId,
                UserId = transaction.UserId,
                CategoryId = transaction.CategoryId,
                Type = transaction.Type,
                Amount = transaction.Amount,
                TransactionDate = transaction.TransactionDate
            };

            return CreatedAtAction(nameof(GetTransaction), new { id = transaction.TransactionId }, returnDto);
        }

        // -------------------------------------------------------------
        // 📌 UPDATE TRANSACTION
        // -------------------------------------------------------------
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTransaction(int id, TransactionUpdateDto dto)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null) return NotFound();

            transaction.CategoryId = dto.CategoryId;
            transaction.Type = dto.Type;
            transaction.Amount = dto.Amount;
            transaction.TransactionDate = dto.TransactionDate;

            await _context.SaveChangesAsync();

            var updatedDto = new TransactionDto
            {
                TransactionId = transaction.TransactionId,
                UserId = transaction.UserId,
                CategoryId = transaction.CategoryId,
                Type = transaction.Type,
                Amount = transaction.Amount,
                TransactionDate = transaction.TransactionDate
            };

            return Ok(updatedDto);
        }

        // -------------------------------------------------------------
        // 📌 DELETE TRANSACTION
        // -------------------------------------------------------------
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null) return NotFound();

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
