using Dapper;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using LM.Orders.Domain.Interfaces;
using LM.SharedKernel.Dtos;

namespace LM.Orders.Infrastructure.Repositories
{
    public class OrderDapperRepository(IConfiguration configuration) : IOrderDapperRepository
    {
        private readonly IConfiguration _configuration = configuration;

        private IDbConnection CreateConnection
        {
            get
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                return new SqlConnection(connectionString);
            }
        }

        private const string BaseSelect = @"
            SELECT
                o.Id, o.CustomerId, o.OrderDate, o.Status, o.TotalAmount, o.CreatedAt, o.CreatedByUserId,
                u.Name AS CreatedByName
            FROM [dbo].[Orders] o WITH(NOLOCK)
            LEFT JOIN [dbo].[User] u WITH(NOLOCK) ON o.CreatedByUserId = u.Id";

        private const string BaseWhere = " WHERE o.Id = @Id";

        public async Task<OrderReadItem?> GetOrderByIdAsync(Guid id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("Id", id);

            var selectSql = $@"{BaseSelect}
                {BaseWhere}";

            using var connection = CreateConnection;

            return await connection.QueryFirstOrDefaultAsync<OrderReadItem>(selectSql, parameters);
        }
    }
}