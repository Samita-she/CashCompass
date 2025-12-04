using CashCompass.API.Models;
using HotChocolate.Types;

namespace CashCompass.API.GraphQL.Types
{
    public class IncomeSourceType : ObjectType<IncomeSource>
    {
        protected override void Configure(IObjectTypeDescriptor<IncomeSource> descriptor)
        {
            descriptor.Field(i => i.IncomeId).Type<NonNullType<IdType>>();
            //descriptor.Field(i => i.UserId).Type<NonNullType<IntType>>();
            descriptor.Field(i => i.UserId).Ignore();
            descriptor.Field(i => i.SourceName).Type<NonNullType<StringType>>();
            descriptor.Field(i => i.Amount).Type<NonNullType<DecimalType>>();
            descriptor.Field(i => i.PayFrequency).Type<NonNullType<StringType>>();
            descriptor.Field(i => i.NextPayDate).Type<NonNullType<DateTimeType>>();
            descriptor.Field(i => i.CreatedAt).Type<NonNullType<DateTimeType>>();
            descriptor.Field(i => i.UserId).Ignore();
            descriptor.Field(i => i.User).Type<UserType>();
            descriptor.Field(i => i.Allocations).Type<ListType<AllocationType>>();
        }
    }
}
