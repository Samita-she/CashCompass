using Microsoft.EntityFrameworkCore;
using CashCompass.API.Models;

namespace CashCompass.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

        public DbSet<User> Users { get; set; }
        public DbSet<IncomeSource> IncomeSources { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Allocation> Allocations { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        // 👇 ADD THIS — FIXES DECIMAL PRECISION WARNINGS
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IncomeSource>()
                .Property(i => i.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Expense>()
                .Property(e => e.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Allocation>()
                .Property(a => a.AllocationValue)
                .HasPrecision(18, 2);
        }
    }
}
