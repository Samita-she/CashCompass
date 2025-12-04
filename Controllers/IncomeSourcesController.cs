using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CashCompass.API.Data;
using CashCompass.API.Models; // for IncomeSource
using CashCompass.API.DTOs;   // for IncomeSourceDto

namespace CashCompass.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IncomeSourcesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public IncomeSourcesController(AppDbContext context)
        {
            _context = context;
        }

        // 📌 Helper to convert entity → DTO
        private IncomeSourceDto ToDto(IncomeSource src)
        {
            return new IncomeSourceDto
            {
                IncomeId = src.IncomeId,
                SourceName = src.SourceName,
                Amount = src.Amount,
                UserId = src.UserId,
                CreatedAt = src.CreatedAt
            };
        }

        // 📌 GET ALL INCOME SOURCES
        [HttpGet]
        public async Task<IActionResult> GetIncomeSources()
        {
            var sources = await _context.IncomeSources.ToListAsync();
            return Ok(sources.Select(s => ToDto(s)));
        }

        // 📌 GET BY USER ID
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(int userId)
        {
            var incomeSources = await _context.IncomeSources
                .Where(i => i.UserId == userId)
                .ToListAsync();

            return Ok(incomeSources.Select(s => ToDto(s)));
        }

        // 📌 GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetIncomeSource(int id)
        {
            var source = await _context.IncomeSources.FindAsync(id);
            if (source == null) return NotFound();

            return Ok(ToDto(source));
        }

        // 📌 CREATE
        [HttpPost]
        public async Task<IActionResult> CreateIncomeSource(IncomeSourceCreateDto dto)
        {
            var newSource = new IncomeSource
            {
                SourceName = dto.SourceName,
                Amount = dto.Amount,
                UserId = dto.UserId,
                
                PayFrequency = dto.PayFrequency,
                NextPayDate = dto.NextPayDate,
            };

            _context.IncomeSources.Add(newSource);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetIncomeSource),
                new { id = newSource.IncomeId },
                ToDto(newSource)
            );
        }

        // 📌 UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateIncomeSource(int id, IncomeSourceUpdateDto dto)
        {
            var source = await _context.IncomeSources.FindAsync(id);
            if (source == null) return NotFound();

            source.SourceName = dto.SourceName;
            source.Amount = dto.Amount;

            await _context.SaveChangesAsync();
            return Ok(ToDto(source));
        }

        // 📌 DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIncomeSource(int id)
        {
            var source = await _context.IncomeSources.FindAsync(id);
            if (source == null) return NotFound();

            _context.IncomeSources.Remove(source);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
