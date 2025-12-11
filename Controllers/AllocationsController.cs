using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CashCompass.API.Data;
using CashCompass.API.Models; // for Allocation
using CashCompass.API.DTOs;   // for AllocationDto

namespace CashCompass.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AllocationsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AllocationsController(AppDbContext context)
        {
            _context = context;
        }

        // Helper method to create a clean AllocationDto projection
        private static AllocationDto ToDto(Allocation a)
        {
            return new AllocationDto
            {
                AllocationId = a.AllocationId,
                IncomeId = a.IncomeId,
                CategoryId = a.CategoryId,
                // ⭐ FIX: Include UserId in the DTO projection
                UserId = a.UserId, 
                AllocationType = a.AllocationType,
                AllocationValue = a.AllocationValue,
                CreatedAt = a.CreatedAt
            };
        }

        // 📌 GET ALL ALLOCATIONS
        [HttpGet]
        public async Task<IActionResult> GetAllocations()
        {
            var allocations = await _context.Allocations
                .Select(a => ToDto(a)) // Using the helper DTO projection
                .ToListAsync();

            return Ok(allocations);
        }

        // 📌 GET ALLOCATIONS BY INCOME SOURCE
        [HttpGet("income/{incomeId}")]
        public async Task<IActionResult> GetByIncome(int incomeId)
        {
            var allocations = await _context.Allocations
                .Where(a => a.IncomeId == incomeId)
                .Select(a => ToDto(a)) // Using the helper DTO projection
                .ToListAsync();

            return Ok(allocations);
        }

        // 📌 GET ALLOCATION BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAllocation(int id)
        {
            var allocation = await _context.Allocations.FindAsync(id);
            if (allocation == null) return NotFound();

            var dto = ToDto(allocation); // Using the helper DTO projection

            return Ok(dto);
        }

        // 📌 CREATE ALLOCATION
        [HttpPost]
        public async Task<IActionResult> CreateAllocation(AllocationCreateDto dto)
        {
            var allocation = new Allocation
            {
                IncomeId = dto.IncomeId,
                CategoryId = dto.CategoryId,
                UserId = dto.UserId,
                AllocationType = dto.AllocationType,
                AllocationValue = dto.AllocationValue,
                CreatedAt = DateTime.UtcNow
            };

            _context.Allocations.Add(allocation);
            await _context.SaveChangesAsync();

            var result = ToDto(allocation); // Using the helper DTO projection

            return CreatedAtAction(nameof(GetAllocation), new { id = allocation.AllocationId }, result);
        }

        // 📌 UPDATE ALLOCATION
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAllocation(int id, AllocationUpdateDto dto)
        {
            var allocation = await _context.Allocations.FindAsync(id);
            if (allocation == null) return NotFound();
            
            // ⭐ CRITICAL FIX: Ensure all fields are mapped from the DTO
            allocation.IncomeId = dto.IncomeId;
            allocation.CategoryId = dto.CategoryId;
            allocation.UserId = dto.UserId; // ⭐ FIX: Add mapping for UserId
            allocation.AllocationType = dto.AllocationType;
            allocation.AllocationValue = dto.AllocationValue;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Allocations.Any(e => e.AllocationId == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            var result = ToDto(allocation); // Using the helper DTO projection

            return Ok(result);
        }

        // 📌 DELETE ALLOCATION
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAllocation(int id)
        {
            var allocation = await _context.Allocations.FindAsync(id);
            if (allocation == null) return NotFound();

            _context.Allocations.Remove(allocation);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}