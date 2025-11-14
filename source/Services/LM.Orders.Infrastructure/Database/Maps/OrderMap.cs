using FluentNHibernate.Mapping;
using LM.Orders.Domain.Aggregates.OrderAggregate;
using LM.SharedKernel.Enums;

namespace LM.Orders.Infrastructure.Database.Maps
{
    public class OrderMap : ClassMap<Order>
    {
        public OrderMap()
        {
            Table("Orders");

            Id(x => x.Id).GeneratedBy.Assigned();

            Map(x => x.CustomerId).Not.Nullable();

            Map(x => x.OrderDate).Not.Nullable();

            Map(x => x.TotalAmount)
                .Precision(18)
                .Scale(2)
                .Not.Nullable();

            Map(x => x.Status)
                .CustomType<OrderStatus>()
                .Not.Nullable();

            Map(x => x.CreatedAt).Not.Nullable();
            Map(x => x.CreatedByUserId).Not.Nullable();

            Map(x => x.UpdatedAt).Nullable();
            Map(x => x.UpdatedByUserId).Nullable(); 

            Map(x => x.DeletedAt).Nullable();
            Map(x => x.DeletedByUserId).Nullable(); 

        }
    }
}