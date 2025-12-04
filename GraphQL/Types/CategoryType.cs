using CashCompass.API.Models;
using HotChocolate.Types;


namespace CashCompass.API.GraphQL.Types
{
    public class CategoryType : ObjectType<Category>
    {
        protected override void Configure(IObjectTypeDescriptor<Category> descriptor)
        {
            descriptor.Field(c => c.CategoryId).Type<NonNullType<IdType>>();
            //descriptor.Field(c => c.UserId).Type<NonNullType<IntType>>();
            descriptor.Field(c => c.UserId).Ignore();
            descriptor.Field(c => c.CategoryName).Type<NonNullType<StringType>>();
            descriptor.Field(c => c.Description).Type<NonNullType<StringType>>();
            descriptor.Field(c => c.User).Type<UserType>();
            descriptor.Field(c => c.Expenses).Type<ListType<ExpenseType>>();
            descriptor.Field(c => c.User).Type<UserType>();
            descriptor.Field(c => c.Expenses).Type<ListType<ExpenseType>>();
            descriptor.Field(c => c.Allocations).Type<ListType<AllocationType>>();
        }
    }
}
