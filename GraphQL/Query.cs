using Dapper;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using CashCompass.API.DTOs;
using CashCompass.API.Models;
using HotChocolate;
using HotChocolate.Resolvers;
using HotChocolate.Types;

namespace CashCompass.API.GraphQL
{
    // Note: Registration of this class as the QueryType is handled in Program.cs/Startup.cs
    public class Query
    {
        private readonly IDbConnection _db;

        public Query(IDbConnection db)
        {
            _db = db;
        }

        // --- 1. ROOT QUERIES (Entry points for client queries) ---

        // Users (Returning DTO for security is correct)
        // Note: No user filtering needed here, as the user is logging in/managing their own account.
        public async Task<UserDto?> GetUser(int userId)
        {
            var sql = "SELECT \"UserId\", \"FullName\", \"Email\", \"CreatedAt\" FROM \"Users\" WHERE \"UserId\" = @UserId";
            // Note on PostgreSQL: Column names are typically case-sensitive and quoted.
            return await _db.QuerySingleOrDefaultAsync<UserDto>(sql, new { UserId = userId });
        }

        // IncomeSources
        [UsePaging]
        public async Task<IEnumerable<IncomeSource>> GetIncomeSources(int userId)
        {
            var sql = "SELECT * FROM \"IncomeSource\" WHERE \"UserId\" = @UserId";
            return await _db.QueryAsync<IncomeSource>(sql, new { UserId = userId });
        }

        public async Task<IncomeSource?> GetIncomeSource(int incomeId)
        {
            var sql = "SELECT * FROM \"IncomeSource\" WHERE \"IncomeId\" = @IncomeId";
            return await _db.QuerySingleOrDefaultAsync<IncomeSource>(sql, new { IncomeId = incomeId });
        }
        
        // Categories
        [UsePaging]
        public async Task<IEnumerable<Category>> GetCategories(int userId)
        {
            var sql = "SELECT * FROM \"Categories\" WHERE \"UserId\" = @UserId";
            return await _db.QueryAsync<Category>(sql, new { UserId = userId });
        }

        public async Task<Category?> GetCategory(int categoryId)
        {
            var sql = "SELECT * FROM \"Categories\" WHERE \"CategoryId\" = @CategoryId";
            return await _db.QuerySingleOrDefaultAsync<Category>(sql, new { CategoryId = categoryId });
        }

        // Expenses
        [UsePaging]
        public async Task<IEnumerable<Expense>> GetExpenses(int userId)
        {
            var sql = "SELECT * FROM \"Expenses\" WHERE \"UserId\" = @UserId";
            return await _db.QueryAsync<Expense>(sql, new { UserId = userId });
        }

        public async Task<Expense?> GetExpense(int expenseId)
        {
            var sql = "SELECT * FROM \"Expenses\" WHERE \"ExpenseId\" = @ExpenseId";
            return await _db.QuerySingleOrDefaultAsync<Expense>(sql, new { ExpenseId = expenseId });
        }

        // Allocations
        [UsePaging]
        public async Task<IEnumerable<Allocation>> GetAllocations(int userId)
        {
            var sql = "SELECT * FROM \"Allocations\" WHERE \"UserId\" = @UserId";
            return await _db.QueryAsync<Allocation>(sql, new { UserId = userId });
        }

        public async Task<Allocation?> GetAllocation(int allocationId)
        {
            var sql = "SELECT * FROM \"Allocations\" WHERE \"AllocationId\" = @AllocationId";
            return await _db.QuerySingleOrDefaultAsync<Allocation>(sql, new { AllocationId = allocationId });
        }

        // --- 2. RELATIONSHIP RESOLVERS (Addressing the N+1 problem using [Parent]) ---

        // Resolvers for Category relationships
        [BindMember(nameof(Category.Expenses))]
        public async Task<IEnumerable<Expense>> GetExpensesForCategory([Parent] Category category)
        {
            var sql = "SELECT * FROM \"Expenses\" WHERE \"CategoryId\" = @CategoryId";
            return await _db.QueryAsync<Expense>(sql, new { CategoryId = category.CategoryId });
        }

        [BindMember(nameof(Category.Allocations))]
        public async Task<IEnumerable<Allocation>> GetAllocationsForCategory([Parent] Category category)
        {
            var sql = "SELECT * FROM \"Allocations\" WHERE \"CategoryId\" = @CategoryId";
            return await _db.QueryAsync<Allocation>(sql, new { CategoryId = category.CategoryId });
        }
        
        // Resolvers for User collections
        [BindMember(nameof(User.Categories))]
        public async Task<IEnumerable<Category>> GetCategoriesForUser([Parent] User user)
        {
            var sql = "SELECT * FROM \"Categories\" WHERE \"UserId\" = @UserId";
            return await _db.QueryAsync<Category>(sql, new { UserId = user.UserId });
        }

        [BindMember(nameof(User.IncomeSources))]
        public async Task<IEnumerable<IncomeSource>> GetIncomeSourcesForUser([Parent] User user)
        {
            var sql = "SELECT * FROM \"IncomeSource\" WHERE \"UserId\" = @UserId";
            return await _db.QueryAsync<IncomeSource>(sql, new { UserId = user.UserId });
        }
        
        [BindMember(nameof(User.Allocations))]
        public async Task<IEnumerable<Allocation>> GetAllocationsForUser([Parent] User user)
        {
            var sql = "SELECT * FROM \"Allocations\" WHERE \"UserId\" = @UserId";
            return await _db.QueryAsync<Allocation>(sql, new { UserId = user.UserId });
        }

        [BindMember(nameof(User.Expenses))]
        public async Task<IEnumerable<Expense>> GetExpensesForUser([Parent] User user)
        {
            var sql = "SELECT * FROM \"Expenses\" WHERE \"UserId\" = @UserId";
            return await _db.QueryAsync<Expense>(sql, new { UserId = user.UserId });
        }

        // Resolvers for Allocation's parent objects
        [BindMember(nameof(Allocation.User))]
        public async Task<UserDto?> GetUserForAllocation([Parent] Allocation allocation)
        {
            return await GetUser(allocation.UserId);
        }

        [BindMember(nameof(Allocation.Category))]
        public async Task<Category?> GetCategoryForAllocation([Parent] Allocation allocation)
        {
            return await GetCategory(allocation.CategoryId);
        }
        
        [BindMember(nameof(Allocation.IncomeSource))]
        public async Task<IncomeSource?> GetIncomeSourceForAllocation([Parent] Allocation allocation)
        {
            var sql = "SELECT * FROM \"IncomeSource\" WHERE \"IncomeId\" = @IncomeId";
            return await _db.QuerySingleOrDefaultAsync<IncomeSource>(sql, new { IncomeId = allocation.IncomeId });
        }

        // Resolvers for Expense's parent objects
        [BindMember(nameof(Expense.User))]
        public async Task<UserDto?> GetUserForExpense([Parent] Expense expense)
        {
            return await GetUser(expense.UserId);
        }

        [BindMember(nameof(Expense.Category))]
        public async Task<Category?> GetCategoryForExpense([Parent] Expense expense)
        {
            return await GetCategory(expense.CategoryId);
        }
        
        // Resolvers for IncomeSource children
        [BindMember(nameof(IncomeSource.Allocations))]
        public async Task<IEnumerable<Allocation>> GetAllocationsForIncomeSource([Parent] IncomeSource incomeSource)
        {
            var sql = "SELECT * FROM \"Allocations\" WHERE \"IncomeId\" = @IncomeId";
            return await _db.QueryAsync<Allocation>(sql, new { IncomeId = incomeSource.IncomeId });
        }

        // Resolvers for Category's parent object
        [BindMember(nameof(Category.User))]
        public async Task<UserDto?> GetUserForCategory([Parent] Category category)
        {
            return await GetUser(category.UserId);
        }
    }
}