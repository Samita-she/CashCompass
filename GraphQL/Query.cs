using Dapper;
using System.Data;
using HotChocolate;
using HotChocolate.Types;
using CashCompass.API.DTOs;
using CashCompass.API.Models;

namespace CashCompass.API.GraphQL
{
    public class Query
    {
        private readonly IDbConnection _db;

        public Query(IDbConnection db)
        {
            _db = db;
        }

        // ==============================
        // Users
        // ==============================
        public async Task<IEnumerable<UserDto>> GetUsers()
        {
            var sql = "SELECT UserId, FullName, Email, CreatedAt FROM Users";
            return await _db.QueryAsync<UserDto>(sql);
        }

        public async Task<UserDto> GetUser(int userId)
        {
            var sql = "SELECT UserId, FullName, Email, CreatedAt FROM Users WHERE UserId = @UserId";
            return await _db.QuerySingleOrDefaultAsync<UserDto>(sql, new { UserId = userId });
        }

        // ==============================
        // IncomeSources
        // ==============================
        public async Task<IEnumerable<IncomeSource>> GetIncomeSources()
        {
            var sql = "SELECT * FROM IncomeSource";
            return await _db.QueryAsync<IncomeSource>(sql);
        }

        public async Task<IncomeSource> GetIncomeSource(int incomeId)
        {
            var sql = "SELECT * FROM IncomeSource WHERE IncomeId = @IncomeId";
            return await _db.QuerySingleOrDefaultAsync<IncomeSource>(sql, new { IncomeId = incomeId });
        }

        // ==============================
        // Categories
        // ==============================
        public async Task<IEnumerable<Category>> GetCategories()
        {
            var sql = "SELECT * FROM Categories";
            return await _db.QueryAsync<Category>(sql);
        }

        public async Task<Category> GetCategory(int categoryId)
        {
            var sql = "SELECT * FROM Categories WHERE CategoryId = @CategoryId";
            return await _db.QuerySingleOrDefaultAsync<Category>(sql, new { CategoryId = categoryId });
        }

        // ==============================
        // Expenses
        // ==============================
        public async Task<IEnumerable<Expense>> GetExpenses()
        {
            var sql = "SELECT * FROM Expenses";
            return await _db.QueryAsync<Expense>(sql);
        }

        public async Task<Expense> GetExpense(int expenseId)
        {
            var sql = "SELECT * FROM Expenses WHERE ExpenseId = @ExpenseId";
            return await _db.QuerySingleOrDefaultAsync<Expense>(sql, new { ExpenseId = expenseId });
        }

        // ==============================
        // Allocations
        // ==============================
        public async Task<IEnumerable<Allocation>> GetAllocations()
        {
            var sql = "SELECT * FROM Allocations";
            return await _db.QueryAsync<Allocation>(sql);
        }

        public async Task<Allocation> GetAllocation(int allocationId)
        {
            var sql = "SELECT * FROM Allocations WHERE AllocationId = @AllocationId";
            return await _db.QuerySingleOrDefaultAsync<Allocation>(sql, new { AllocationId = allocationId });
        }

        // ==============================
        // Transactions
        // ==============================
        public async Task<IEnumerable<Transaction>> GetTransactions()
        {
            var sql = "SELECT * FROM Transactions";
            return await _db.QueryAsync<Transaction>(sql);
        }

        public async Task<Transaction> GetTransaction(int transactionId)
        {
            var sql = "SELECT * FROM Transactions WHERE TransactionId = @TransactionId";
            return await _db.QuerySingleOrDefaultAsync<Transaction>(sql, new { TransactionId = transactionId });
        }
    }
}
