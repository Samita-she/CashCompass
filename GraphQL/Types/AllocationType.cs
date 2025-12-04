using CashCompass.API.Models;
using HotChocolate.Types;

namespace CashCompass.API.GraphQL.Types
{
    public class AllocationType : ObjectType<Allocation>
    {
        protected override void Configure(IObjectTypeDescriptor<Allocation> descriptor)
        {
            descriptor.Field(a => a.AllocationId).Type<NonNullType<IdType>>();
            descriptor.Field(a => a.IncomeId).Type<NonNullType<IntType>>();
            descriptor.Field(a => a.CategoryId).Type<NonNullType<IntType>>();
            descriptor.Field(a => a.AllocationType).Type<NonNullType<StringType>>();
            descriptor.Field(a => a.AllocationValue).Type<NonNullType<DecimalType>>();
            descriptor.Field(a => a.CreatedAt).Type<NonNullType<DateTimeType>>();
            descriptor.Field(a => a.UserId).Type<NonNullType<IntType>>().Ignore();
            descriptor.Field(a => a.IncomeId).Ignore(); 
            descriptor.Field(a => a.CategoryId).Ignore();
            descriptor.Field(a => a.User).Type<UserType>();
            descriptor.Field(a => a.IncomeSource).Type<IncomeSourceType>();
            descriptor.Field(a => a.Category).Type<CategoryType>();
        }
    }
}
