using OrderSubmissionSystem.Application.Interfaces;
using OrderSubmissionSystem.Domain.Entities;
using Serilog;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace OrderSubmissionSystem.Infrastructure.Processors
{
    public class SqlOrderProcessor : IOrderProcessor
    {
        private readonly string _connectionString;
        private readonly ILogger _logger;

        public SqlOrderProcessor()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["OrderDb"]?.ConnectionString
                ?? throw new InvalidOperationException("OrderDb connection string not found");
            _logger = Log.ForContext<SqlOrderProcessor>();
        }

        public async Task<bool> ProcessOrderAsync(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync().ConfigureAwait(false);

                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            var orderCommand = new SqlCommand(
                                @"INSERT INTO Orders (OrderId, CustomerId, TotalAmount, OrderDate)
                          VALUES (@OrderId, @CustomerId, @TotalAmount, @OrderDate)",
                                connection, transaction);

                            orderCommand.Parameters.AddWithValue("@OrderId", order.OrderId);
                            orderCommand.Parameters.AddWithValue("@CustomerId", order.CustomerId);
                            orderCommand.Parameters.AddWithValue("@TotalAmount", order.TotalAmount);
                            orderCommand.Parameters.AddWithValue("@OrderDate", order.OrderDate);

                            await orderCommand.ExecuteNonQueryAsync().ConfigureAwait(false);

                            foreach (var item in order.Items)
                            {
                                var itemCommand = new SqlCommand(
                                    @"INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice)
                              VALUES (@OrderId, @ProductId, @Quantity, @UnitPrice)",
                                    connection, transaction);

                                itemCommand.Parameters.AddWithValue("@OrderId", order.OrderId);
                                itemCommand.Parameters.AddWithValue("@ProductId", item.ProductId);
                                itemCommand.Parameters.AddWithValue("@Quantity", item.Quantity);
                                itemCommand.Parameters.AddWithValue("@UnitPrice", item.UnitPrice);

                                await itemCommand.ExecuteNonQueryAsync().ConfigureAwait(false);
                            }

                            transaction.Commit();
                            _logger.Information("Order {OrderId} persisted to SQL", order.OrderId);
                            return true;
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error processing order {OrderId} via SQL", order?.OrderId);
                return false;
            }
        }
    }
}
