using Dapper;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using CashCompass.API.DTOs;
using CashCompass.API.Models;
using HotChocolate;
using Npgsql; 

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
        public async Task<UserDto?> CreateUser(string fullName, string email, string password)
{
    try
    {
        var result = await _db.QuerySingleAsync<UserDto>(
            "create_user", 
            new { FullName = fullName, Email = email, Password = password },
            commandType: CommandType.StoredProcedure
        );

        return result;
    }
    catch (Exception ex)
    {
        // TODO: Replace with your logging framework
        Console.WriteLine($"[ERROR] Failed to create user: {ex.Message}");

        // You can rethrow, wrap, or return null based on your design
        return null;
    }
}


        public async Task<UserDto> UpdateUser(int userId, string fullName, string email)
        {
            return await _db.QuerySingleAsync<UserDto>(
                "update_user",
                new { UserId = userId, FullName = fullName, Email = email },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<bool> DeleteUser(int userId)
        {
           
            await _db.ExecuteAsync(
                "delete_user",
                new { UserId = userId },
                commandType: CommandType.StoredProcedure
            );
            return true;
        }

      
        
        // ==============================
        // IncomeSources
        // ==============================
        public async Task<IncomeSource> CreateIncomeSource(int userId, string sourceName, decimal amount, string payFrequency, DateTime nextPayDate)
        {
            return await _db.QuerySingleAsync<IncomeSource>(
                "create_income_source",
                new { UserId = userId, SourceName = sourceName, Amount = amount, PayFrequency = payFrequency, NextPayDate = nextPayDate },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<IncomeSource> UpdateIncomeSource(int incomeId, string sourceName, decimal amount, string payFrequency, DateTime nextPayDate)
        {
            return await _db.QuerySingleAsync<IncomeSource>(
                "update_income_source",
                new { IncomeId = incomeId, SourceName = sourceName, Amount = amount, PayFrequency = payFrequency, NextPayDate = nextPayDate },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<bool> DeleteIncomeSource(int incomeId)
        {
            await _db.ExecuteAsync(
                "delete_income_source",
                new { IncomeId = incomeId },
                commandType: CommandType.StoredProcedure
            );
            return true;
        }

        // ==============================
        // Categories
        // ==============================
        public async Task<Category> CreateCategory(int userId, string categoryName, string description)
        {
            return await _db.QuerySingleAsync<Category>(
                "create_category",
                new { UserId = userId, CategoryName = categoryName, Description = description },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<Category> UpdateCategory(int categoryId, string categoryName, string description)
        {
            return await _db.QuerySingleAsync<Category>(
                "update_category",
                new { CategoryId = categoryId, CategoryName = categoryName, Description = description },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<bool> DeleteCategory(int categoryId)
        {
            await _db.ExecuteAsync(
                "delete_category",
                new { CategoryId = categoryId },
                commandType: CommandType.StoredProcedure
            );
            return true;
        }

        // ==============================
        // Expenses
        // ==============================
        public async Task<Expense> CreateExpense(int userId, string expenseName, decimal amount, int categoryId, DateTime expenseDate, string notes)
        {
            return await _db.QuerySingleAsync<Expense>(
                "create_expense",
                new { UserId = userId, ExpenseName = expenseName, Amount = amount, CategoryId = categoryId, ExpenseDate = expenseDate, Notes = notes },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<Expense> UpdateExpense(int expenseId, string expenseName, decimal amount, int categoryId, DateTime expenseDate, string notes)
        {
            return await _db.QuerySingleAsync<Expense>(
                "update_expense",
                new { ExpenseId = expenseId, ExpenseName = expenseName, Amount = amount, CategoryId = categoryId, ExpenseDate = expenseDate, Notes = notes },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<bool> DeleteExpense(int expenseId)
        {
            await _db.ExecuteAsync(
                "delete_expense",
                new { ExpenseId = expenseId },
                commandType: CommandType.StoredProcedure
            );
            return true;
        }

        // ==============================
        // Allocations (CRITICAL FIX: Added UserId)
        // ==============================
        public async Task<Allocation> CreateAllocation(int userId, int incomeId, int categoryId, string allocationType, decimal allocationValue)
        {
            return await _db.QuerySingleAsync<Allocation>(
                "create_allocation",
                new { UserId = userId, IncomeId = incomeId, CategoryId = categoryId, AllocationType = allocationType, AllocationValue = allocationValue },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<Allocation> UpdateAllocation(int allocationId, string allocationType, decimal allocationValue)
        {
            // Note: UserId and IncomeId are typically not updated in this mutation.
            return await _db.QuerySingleAsync<Allocation>(
                "update_allocation",
                new { AllocationId = allocationId, AllocationType = allocationType, AllocationValue = allocationValue },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<bool> DeleteAllocation(int allocationId)
        {
            await _db.ExecuteAsync(
                "delete_allocation",
                new { AllocationId = allocationId },
                commandType: CommandType.StoredProcedure
            );
            return true;
        }
    }
}