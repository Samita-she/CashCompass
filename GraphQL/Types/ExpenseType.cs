using CashCompass.API.Models;
using HotChocolate.Types;

namespace CashCompass.API.GraphQL.Types
{
    public class ExpenseType : ObjectType<Expense>
    {
        protected override void Configure(IObjectTypeDescriptor<Expense> descriptor)
        {
            descriptor.Field(e => e.ExpenseId).Type<NonNullType<IdType>>();
            //descriptor.Field(e => e.CategoryId).Type<NonNullType<IntType>>();
            //descriptor.Field(e => e.UserId).Type<NonNullType<IntType>>();
            descriptor.Field(e => e.UserId).Ignore(); 
            descriptor.Field(e => e.CategoryId).Ignore();
            descriptor.Field(e => e.ExpenseName).Type<NonNullType<StringType>>();
            descriptor.Field(e => e.Amount).Type<NonNullType<DecimalType>>();
            descriptor.Field(e => e.ExpenseDate).Type<NonNullType<DateTimeType>>();
            descriptor.Field(e => e.Notes).Type<StringType>();
            descriptor.Field(e => e.CreatedAt).Type<NonNullType<DateTimeType>>();
            descriptor.Field(e => e.User).Type<UserType>();
            descriptor.Field(e => e.Category).Type<CategoryType>();
        }
    }
}
