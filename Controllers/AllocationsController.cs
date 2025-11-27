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

        // 📌 GET ALL ALLOCATIONS
        [HttpGet]
        public async Task<IActionResult> GetAllocations()
        {
            var allocations = await _context.Allocations
                .Select(a => new AllocationDto
                {
                    AllocationId = a.AllocationId,
                    IncomeId = a.IncomeId,
                    CategoryId = a.CategoryId,
                    AllocationType = a.AllocationType,
                    AllocationValue = a.AllocationValue,
                    CreatedAt = a.CreatedAt
                })
                .ToListAsync();

            return Ok(allocations);
        }

        // 📌 GET ALLOCATIONS BY INCOME SOURCE
        [HttpGet("income/{incomeId}")]
        public async Task<IActionResult> GetByIncome(int incomeId)
        {
            var allocations = await _context.Allocations
                .Where(a => a.IncomeId == incomeId)
                .Select(a => new AllocationDto
                {
                    AllocationId = a.AllocationId,
                    IncomeId = a.IncomeId,
                    CategoryId = a.CategoryId,
                    AllocationType = a.AllocationType,
                    AllocationValue = a.AllocationValue,
                    CreatedAt = a.CreatedAt
                })
                .ToListAsync();

            return Ok(allocations);
        }

        // 📌 GET ALLOCATION BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAllocation(int id)
        {
            var allocation = await _context.Allocations.FindAsync(id);
            if (allocation == null) return NotFound();

            var dto = new AllocationDto
            {
                AllocationId = allocation.AllocationId,
                IncomeId = allocation.IncomeId,
                CategoryId = allocation.CategoryId,
                AllocationType = allocation.AllocationType,
                AllocationValue = allocation.AllocationValue,
                CreatedAt = allocation.CreatedAt
            };

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
                AllocationType = dto.AllocationType,
                AllocationValue = dto.AllocationValue,
                CreatedAt = DateTime.UtcNow
            };

            _context.Allocations.Add(allocation);
            await _context.SaveChangesAsync();

            var result = new AllocationDto
            {
                AllocationId = allocation.AllocationId,
                IncomeId = allocation.IncomeId,
                CategoryId = allocation.CategoryId,
                AllocationType = allocation.AllocationType,
                AllocationValue = allocation.AllocationValue,
                CreatedAt = allocation.CreatedAt
            };

            return CreatedAtAction(nameof(GetAllocation), new { id = allocation.AllocationId }, result);
        }

        // 📌 UPDATE ALLOCATION
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAllocation(int id, AllocationUpdateDto dto)
        {
            var allocation = await _context.Allocations.FindAsync(id);
            if (allocation == null) return NotFound();

            allocation.CategoryId = dto.CategoryId;
            allocation.AllocationType = dto.AllocationType;
            allocation.AllocationValue = dto.AllocationValue;

            await _context.SaveChangesAsync();

            var result = new AllocationDto
            {
                AllocationId = allocation.AllocationId,
                IncomeId = allocation.IncomeId,
                CategoryId = allocation.CategoryId,
                AllocationType = allocation.AllocationType,
                AllocationValue = allocation.AllocationValue,
                CreatedAt = allocation.CreatedAt
            };

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
