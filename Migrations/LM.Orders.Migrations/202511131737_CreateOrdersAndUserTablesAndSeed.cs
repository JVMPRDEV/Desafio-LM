using FluentMigrator;

namespace LM.Orders.Migrations
{
    [Migration(202511131737)]
    public class CreateOrdersAndUserTablesAndSeed : Migration
    {
        private const string OrdersTableName = "Orders";
        private const string UserTableName = "User";
        private readonly Guid FixedUserId = new("3fa85f64-5717-4562-b3fc-2c963f66afa6");

        public override void Up()
        {
            Create.Table(UserTableName)
                .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
                .WithColumn("Name").AsString(150).NotNullable()
                .WithColumn("Email").AsString(150).NotNullable().Unique()
                .WithColumn("CreatedAt").AsDateTime().NotNullable();

            Insert.IntoTable(UserTableName).Row(new
            {
                Id = FixedUserId,
                Name = "System User",
                Email = "system@lmorders.com.br",
                CreatedAt = DateTime.Now
            });

            Create.Table(OrdersTableName)
                .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
                .WithColumn("CustomerId").AsGuid().NotNullable()
                .WithColumn("OrderDate").AsDateTime().NotNullable()
                .WithColumn("Status").AsString(50).NotNullable()
                .WithColumn("TotalAmount").AsDecimal(18, 2).NotNullable()

                .WithColumn("CreatedAt").AsDateTime().NotNullable()
                .WithColumn("CreatedByUserId").AsGuid().NotNullable()
                .WithColumn("UpdatedAt").AsDateTime().Nullable()
                .WithColumn("UpdatedByUserId").AsGuid().Nullable()
                .WithColumn("DeletedAt").AsDateTime().Nullable()
                .WithColumn("DeletedByUserId").AsGuid().Nullable();

            Create.Index("IX_Orders_CustomerId").OnTable(OrdersTableName).OnColumn("CustomerId");
        }

        public override void Down()
        {
            Delete.Table(OrdersTableName);
            Delete.Table(UserTableName);
        }
    }
}