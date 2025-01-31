using ECommerceAPI.DTO;
using ECommerceAPI.Models;
using Microsoft.Data.SqlClient;

namespace ECommerceAPI.Data
{
    public class OrderRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public OrderRepository(SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<List<DetailedOrderDTO>> GetAllOrdersAsync()
        {
            var orders = new List<DetailedOrderDTO>();
            var query = @"
                SELECT 
                    o.OrderId, o.TotalAmount, o.Status, o.OrderDate,
                    c.CustomerId, c.Name AS CustomerName, c.Email, c.Address, c.Role,
                    p.ProductId, p.Name AS ProductName, p.Description AS ProductDescription, 
                    oi.Quantity, oi.PriceAtOrder
                FROM Orders o
                JOIN Customers c ON o.CustomerId = c.CustomerId
                JOIN OrderItems oi ON o.OrderId = oi.OrderId
                JOIN Products p ON oi.ProductId = p.ProductId";

            using (var connection = _connectionFactory.CreateConnection())
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var orderId = reader.GetInt32(reader.GetOrdinal("OrderId"));

                            var existingOrder = orders.Find(o => o.OrderId == orderId);
                            if (existingOrder == null)
                            {
                                existingOrder = new DetailedOrderDTO
                                {
                                    OrderId = orderId,
                                    TotalAmount = reader.GetDecimal(reader.GetOrdinal("TotalAmount")),
                                    Status = reader.GetString(reader.GetOrdinal("Status")),
                                    OrderDate = reader.GetDateTime(reader.GetOrdinal("OrderDate")),
                                    Customer = new CustomerDTO
                                    {
                                        CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                        Name = reader.GetString(reader.GetOrdinal("CustomerName")),
                                        Email = reader.GetString(reader.GetOrdinal("Email")),
                                        Address = reader.GetString(reader.GetOrdinal("Address")),
                                        Role = reader.GetString(reader.GetOrdinal("Role"))
                                    },
                                    Items = new List<DetailedOrderItemDTO>()
                                };
                                orders.Add(existingOrder);
                            }

                            existingOrder.Items.Add(new DetailedOrderItemDTO
                            {
                                ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                                ProductName = reader.GetString(reader.GetOrdinal("ProductName")),
                                ProductDescription = reader.GetString(reader.GetOrdinal("ProductDescription")),
                                PriceAtOrder = reader.GetDecimal(reader.GetOrdinal("PriceAtOrder")),
                                Quantity = reader.GetInt32(reader.GetOrdinal("Quantity"))
                            });
                        }
                    }
                }
            }

            return orders;
        }



        public async Task<CreateOrderResponseDTO> CreateOrderAsync(OrderDTO orderDto)
        {
            var productQuery = "SELECT ProductId, Price, Quantity FROM Products WHERE ProductId = @ProductId AND IsDeleted = 0";
            var orderQuery = "INSERT INTO Orders (CustomerId, TotalAmount, Status, OrderDate) OUTPUT INSERTED.OrderId VALUES (@CustomerId, @TotalAmount, @Status, @OrderDate)";
            var itemQuery = "INSERT INTO OrderItems (OrderId, ProductId, Quantity, PriceAtOrder) VALUES (@OrderId, @ProductId, @Quantity, @PriceAtOrder)";

            decimal totalAmount = 0m;
            List<OrderItem> validatedItems = new List<OrderItem>();

            CreateOrderResponseDTO createOrderResponseDTO = new CreateOrderResponseDTO();

            using (var connection = _connectionFactory.CreateConnection())
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (OrderItemDetailsDTO item in orderDto.Items)
                        {
                            using (var productCommand = new SqlCommand(productQuery, connection, transaction))
                            {
                                productCommand.Parameters.AddWithValue("@ProductId", item.ProductId);
                                using (var reader = await productCommand.ExecuteReaderAsync())
                                {
                                    if (await reader.ReadAsync())
                                    {
                                        int stockQuantity = reader.GetInt32(reader.GetOrdinal("Quantity"));
                                        decimal price = reader.GetDecimal(reader.GetOrdinal("Price"));

                                        if (stockQuantity >= item.Quantity)
                                        {
                                            totalAmount += price * item.Quantity;
                                            validatedItems.Add(new OrderItem
                                            {
                                                ProductId = item.ProductId,
                                                Quantity = item.Quantity,
                                                PriceAtOrder = price
                                            });
                                        }
                                        else
                                        {
                                            createOrderResponseDTO.Message = $"Insufficient Stock for Product ID {item.ProductId}";
                                            createOrderResponseDTO.IsCreated = false;
                                            return createOrderResponseDTO;
                                        }
                                    }
                                    else
                                    {
                                        createOrderResponseDTO.Message = $"Product Not Found for Product ID {item.ProductId}";
                                        createOrderResponseDTO.IsCreated = false;
                                        return createOrderResponseDTO;
                                    }
                                    reader.Close();
                                }
                            }
                        }

                        using (var orderCommand = new SqlCommand(orderQuery, connection, transaction))
                        {
                            orderCommand.Parameters.AddWithValue("@CustomerId", orderDto.CustomerId);
                            orderCommand.Parameters.AddWithValue("@TotalAmount", totalAmount);
                            orderCommand.Parameters.AddWithValue("@Status", "Pending");
                            orderCommand.Parameters.AddWithValue("@OrderDate", DateTime.Now);

                            var orderId = (int)await orderCommand.ExecuteScalarAsync();

                            foreach (var validatedItem in validatedItems)
                            {
                                using (var itemCommand = new SqlCommand(itemQuery, connection, transaction))
                                {
                                    itemCommand.Parameters.AddWithValue("@OrderId", orderId);
                                    itemCommand.Parameters.AddWithValue("@ProductId", validatedItem.ProductId);
                                    itemCommand.Parameters.AddWithValue("@Quantity", validatedItem.Quantity);
                                    itemCommand.Parameters.AddWithValue("@PriceAtOrder", validatedItem.PriceAtOrder);

                                    await itemCommand.ExecuteNonQueryAsync();
                                }
                            }

                            transaction.Commit();
                            createOrderResponseDTO.Status = "Pending";
                            createOrderResponseDTO.IsCreated = true;
                            createOrderResponseDTO.OrderId = orderId;
                            createOrderResponseDTO.Message = "Order Created Successfully";
                            return createOrderResponseDTO;
                        }
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
      
        public async Task<bool> DeleteOrderAsync(int orderId)
        {
            var deleteOrderItemsQuery = "DELETE FROM OrderItems WHERE OrderId = @OrderId";
            var deleteOrderQuery = "DELETE FROM Orders WHERE OrderId = @OrderId";

            using (var connection = _connectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        using (var command = new SqlCommand(deleteOrderItemsQuery, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@OrderId", orderId);
                            await command.ExecuteNonQueryAsync();
                        }

                        using (var command = new SqlCommand(deleteOrderQuery, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@OrderId", orderId);
                            var rowsAffected = await command.ExecuteNonQueryAsync();

                            if (rowsAffected > 0)
                            {
                                transaction.Commit();
                                return true;
                            }
                            else
                            {
                                transaction.Rollback();
                                return false;
                            }
                        }
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw; 
                    }
                }
            }
        }

    }
}
