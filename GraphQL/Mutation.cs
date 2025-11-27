using Dapper;
using System.Data;
using HotChocolate;
using HotChocolate.Types;
using CashCompass.API.DTOs;
using CashCompass.API.Models;

namespace CashCompass.API.GraphQL
{
    public class Mutation
    {
        private readonly IDbConnection _db;

        public Mutation(IDbConnection db)
        {
            _db = db;
        }

        // ==============================
        // Users
        // ==============================
        public async Task<UserDto> CreateUser(string fullName, string email, string password)
        {
            var sql = "EXEC CreateUser @FullName, @Email, @Password";
            return await _db.QuerySingleAsync<UserDto>(sql, new { FullName = fullName, Email = email, Password = password });
        }

        public async Task<UserDto> UpdateUser(int userId, string fullName, string email)
        {
            var sql = "EXEC UpdateUser @UserId, @FullName, @Email";
            return await _db.QuerySingleAsync<UserDto>(sql, new { UserId = userId, FullName = fullName, Email = email });
        }

        public async Task<bool> DeleteUser(int userId)
        {
            var sql = "EXEC DeleteUser @UserId";
            await _db.ExecuteAsync(sql, new { UserId = userId });
            return true;
        }

        // Bulk update users
        public async Task<bool> BulkUpdateUsers(List<int> userIds, bool isActive)
        {
            var table = new DataTable();
            table.Columns.Add("Id", typeof(int));
            foreach (var id in userIds)
                table.Rows.Add(id);

            var param = new DynamicParameters();
            param.Add("@UserIds", table.AsTableValuedParameter("dbo.IntList"));
            param.Add("@Status", isActive);

            await _db.ExecuteAsync("BulkUpdateUserStatus", param, commandType: CommandType.StoredProcedure);
            return true;
        }

        // ==============================
        // IncomeSources
        // ==============================
        public async Task<IncomeSource> CreateIncomeSource(int userId, string sourceName, decimal amount, string payFrequency, DateTime nextPayDate)
        {
            var sql = "EXEC CreateIncomeSource @UserId, @SourceName, @Amount, @PayFrequency, @NextPayDate";
            return await _db.QuerySingleAsync<IncomeSource>(sql, new { UserId = userId, SourceName = sourceName, Amount = amount, PayFrequency = payFrequency, NextPayDate = nextPayDate });
        }

        public async Task<IncomeSource> UpdateIncomeSource(int incomeId, string sourceName, decimal amount, string payFrequency, DateTime nextPayDate)
        {
            var sql = "EXEC UpdateIncomeSource @IncomeId, @SourceName, @Amount, @PayFrequency, @NextPayDate";
            return await _db.QuerySingleAsync<IncomeSource>(sql, new { IncomeId = incomeId, SourceName = sourceName, Amount = amount, PayFrequency = payFrequency, NextPayDate = nextPayDate });
        }

        public async Task<bool> DeleteIncomeSource(int incomeId)
        {
            var sql = "EXEC DeleteIncomeSource @IncomeId";
            await _db.ExecuteAsync(sql, new { IncomeId = incomeId });
            return true;
        }

        // ==============================
        // Categories
        // ==============================
        public async Task<Category> CreateCategory(int userId, string categoryName, string description)
        {
            var sql = "EXEC CreateCategory @UserId, @CategoryName, @Description";
            return await _db.QuerySingleAsync<Category>(sql, new { UserId = userId, CategoryName = categoryName, Description = description });
        }

        public async Task<Category> UpdateCategory(int categoryId, string categoryName, string description)
        {
            var sql = "EXEC UpdateCategory @CategoryId, @CategoryName, @Description";
            return await _db.QuerySingleAsync<Category>(sql, new { CategoryId = categoryId, CategoryName = categoryName, Description = description });
        }

        public async Task<bool> DeleteCategory(int categoryId)
        {
            var sql = "EXEC DeleteCategory @CategoryId";
            await _db.ExecuteAsync(sql, new { CategoryId = categoryId });
            return true;
        }

        // ==============================
        // Expenses
        // ==============================
        public async Task<Expense> CreateExpense(int userId, string expenseName, decimal amount, int categoryId, DateTime expenseDate, string notes)
        {
            var sql = "EXEC CreateExpense @UserId, @ExpenseName, @Amount, @CategoryId, @ExpenseDate, @Notes";
            return await _db.QuerySingleAsync<Expense>(sql, new { UserId = userId, ExpenseName = expenseName, Amount = amount, CategoryId = categoryId, ExpenseDate = expenseDate, Notes = notes });
        }

        public async Task<Expense> UpdateExpense(int expenseId, string expenseName, decimal amount, int categoryId, DateTime expenseDate, string notes)
        {
            var sql = "EXEC UpdateExpense @ExpenseId, @ExpenseName, @Amount, @CategoryId, @ExpenseDate, @Notes";
            return await _db.QuerySingleAsync<Expense>(sql, new { ExpenseId = expenseId, ExpenseName = expenseName, Amount = amount, CategoryId = categoryId, ExpenseDate = expenseDate, Notes = notes });
        }

        public async Task<bool> DeleteExpense(int expenseId)
        {
            var sql = "EXEC DeleteExpense @ExpenseId";
            await _db.ExecuteAsync(sql, new { ExpenseId = expenseId });
            return true;
        }

        // ==============================
        // Allocations
        // ==============================
        public async Task<Allocation> CreateAllocation(int incomeId, int categoryId, string allocationType, decimal allocationValue)
        {
            var sql = "EXEC CreateAllocation @IncomeId, @CategoryId, @AllocationType, @AllocationValue";
            return await _db.QuerySingleAsync<Allocation>(sql, new { IncomeId = incomeId, CategoryId = categoryId, AllocationType = allocationType, AllocationValue = allocationValue });
        }

        public async Task<Allocation> UpdateAllocation(int allocationId, string allocationType, decimal allocationValue)
        {
            var sql = "EXEC UpdateAllocation @AllocationId, @AllocationType, @AllocationValue";
            return await _db.QuerySingleAsync<Allocation>(sql, new { AllocationId = allocationId, AllocationType = allocationType, AllocationValue = allocationValue });
        }

        public async Task<bool> DeleteAllocation(int allocationId)
        {
            var sql = "EXEC DeleteAllocation @AllocationId";
            await _db.ExecuteAsync(sql, new { AllocationId = allocationId });
            return true;
        }

        // ==============================
        // Transactions
        // ==============================
        public async Task<Transaction> CreateTransaction(int userId, decimal amount, string transactionType, DateTime transactionDate, string notes)
        {
            var sql = "EXEC CreateTransaction @UserId, @Amount, @TransactionType, @TransactionDate, @Notes";
            return await _db.QuerySingleAsync<Transaction>(sql, new { UserId = userId, Amount = amount, TransactionType = transactionType, TransactionDate = transactionDate, Notes = notes });
        }

        public async Task<Transaction> UpdateTransaction(int transactionId, decimal amount, string transactionType, DateTime transactionDate, string notes)
        {
            var sql = "EXEC UpdateTransaction @TransactionId, @Amount, @TransactionType, @TransactionDate, @Notes";
            return await _db.QuerySingleAsync<Transaction>(sql, new { TransactionId = transactionId, Amount = amount, TransactionType = transactionType, TransactionDate = transactionDate, Notes = notes });
        }

        public async Task<bool> DeleteTransaction(int transactionId)
        {
            var sql = "EXEC DeleteTransaction @TransactionId";
            await _db.ExecuteAsync(sql, new { TransactionId = transactionId });
            return true;
        }
    }
}
