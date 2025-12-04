using CashCompass.API.Models;
using HotChocolate.Types;

namespace CashCompass.API.GraphQL.Types
{
    public class UserType : ObjectType<User>
    {
        protected override void Configure(IObjectTypeDescriptor<User> descriptor)
        {
            descriptor.Description("Represents a user in the CashCompass system.");

            descriptor.Field(u => u.UserId).Type<NonNullType<IdType>>();
            descriptor.Field(u => u.FullName).Type<NonNullType<StringType>>();
            descriptor.Field(u => u.Email).Type<NonNullType<StringType>>();
            descriptor.Field(u => u.PasswordHash).Ignore(); 
            descriptor.Field(u => u.CreatedAt).Type<NonNullType<DateTimeType>>();
            descriptor.Field(u => u.Categories).Type<ListType<CategoryType>>();
            descriptor.Field(u => u.IncomeSources).Type<ListType<IncomeSourceType>>();
            descriptor.Field(u => u.Allocations).Type<ListType<AllocationType>>();
            descriptor.Field(u => u.Expenses).Type<ListType<ExpenseType>>();
        }
    }
}
