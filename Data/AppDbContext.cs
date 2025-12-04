using Microsoft.EntityFrameworkCore;
using CashCompass.API.Models;

namespace CashCompass.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

        // --- DbSet Properties (Tables) ---
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<IncomeSource> IncomeSources { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Allocation> Allocations { get; set; } = null!;
        public DbSet<Expense> Expenses { get; set; } = null!;
        // public DbSet<Transaction> Transactions { get; set; } // ❌ REMOVED per project decision

        // --- Model Configuration and Constraints ---
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. User Configuration
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique(); // 🔑 Ensure emails are unique for registration/login

            // 2. Monetary Precision (Using NUMERIC(18, 2) in PostgreSQL)
            modelBuilder.Entity<IncomeSource>()
                .Property(i => i.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Allocation>()
                .Property(a => a.AllocationValue)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Expense>()
                .Property(e => e.Amount)
                .HasPrecision(18, 2);
            
            // 3. One-to-Many Relationship Configuration (Ownership)

            // User owns Categories
            modelBuilder.Entity<Category>()
                .HasOne(c => c.User)
                .WithMany(u => u.Categories)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade); // If User is deleted, delete their Categories
            
            // User owns IncomeSources
            modelBuilder.Entity<IncomeSource>()
                .HasOne(i => i.User)
                .WithMany(u => u.IncomeSources)
                .HasForeignKey(i => i.UserId)
                .OnDelete(DeleteBehavior.Cascade); 
            
            // Allocation links three models: User, IncomeSource, Category
            modelBuilder.Entity<Allocation>()
                .HasOne(a => a.User)
                .WithMany(u => u.Allocations)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade); 
            
            // Configuration for the other Allocation FKs (IncomeSource and Category)
            modelBuilder.Entity<Allocation>()
                .HasOne(a => a.IncomeSource)
                .WithMany(i => i.Allocations)
                .HasForeignKey(a => a.IncomeId)
                .OnDelete(DeleteBehavior.Cascade); // Deleting IncomeSource removes Allocations from it

            modelBuilder.Entity<Allocation>()
                .HasOne(a => a.Category)
                .WithMany(c => c.Allocations)
                .HasForeignKey(a => a.CategoryId)
                .OnDelete(DeleteBehavior.Restrict); // 🔥 IMPORTANT: Restrict deleting Category if Allocations exist

            // Expense links User and Category
            modelBuilder.Entity<Expense>()
                .HasOne(e => e.User)
                .WithMany(u => u.Expenses)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Expense>()
                .HasOne(e => e.Category)
                .WithMany(c => c.Expenses)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict); // 🔥 IMPORTANT: Restrict deleting Category if Expenses exist
        }
    }
}