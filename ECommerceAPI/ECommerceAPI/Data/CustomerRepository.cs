using ECommerceAPI.DTO;
using ECommerceAPI.Models;
using Microsoft.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace ECommerceAPI.Data
{
    public class CustomerRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public CustomerRepository(SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            var customers = new List<Customer>();
            var query = "SELECT CustomerId, Name, Email, Address FROM Customers WHERE IsDeleted = 0";

            using (var connection = _connectionFactory.CreateConnection())
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            customers.Add(new Customer
                            {
                                CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                Address = reader.GetString(reader.GetOrdinal("Address")),
                                IsDeleted = false
                            });
                        }
                    }
                }
            }

            return customers;
        }

        public async Task<Customer?> GetCustomerByIdAsync(int customerId)
        {
            var query = "SELECT CustomerId, Name, Email, Address,role FROM Customers WHERE CustomerId = @CustomerId AND IsDeleted = 0";
            Customer? customer = null;

            using (var connection = _connectionFactory.CreateConnection())
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CustomerId", customerId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            customer = new Customer
                            {
                                CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                Address = reader.GetString(reader.GetOrdinal("Address")),
                                Role = reader.GetString(reader.GetOrdinal("role")),
                                IsDeleted = false
                            };
                        }
                    }
                }
            }

            return customer;
        }

        public async Task<int> InsertCustomerAsync(CustomerDTO customer)
        {
            var query = @"INSERT INTO Customers (Name, Email, Address, PasswordHash, Role, IsDeleted) 
                        VALUES (@Name, @Email, @Address, @PasswordHash, @Role, 0);
                        SELECT CAST(SCOPE_IDENTITY() as int);";

            using (var connection = _connectionFactory.CreateConnection())
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", customer.Name);
                    command.Parameters.AddWithValue("@Email", customer.Email);
                    command.Parameters.AddWithValue("@Address", customer.Address ?? string.Empty);
                    command.Parameters.AddWithValue("@PasswordHash", HashPassword(customer.Role));
                    command.Parameters.AddWithValue("@Role", customer.Role);

                    int customerId = (int)await command.ExecuteScalarAsync();
                    return customerId;
                }
            }
        }

        public async Task UpdateCustomerAsync(CustomerDTO customer)
        {
            var query = "UPDATE Customers SET Name = @Name, Email = @Email, Address = @Address WHERE CustomerId = @CustomerId";

            using (var connection = _connectionFactory.CreateConnection())
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CustomerId", customer.CustomerId);
                    command.Parameters.AddWithValue("@Name", customer.Name);
                    command.Parameters.AddWithValue("@Email", customer.Email);
                    command.Parameters.AddWithValue("@Address", customer.Address);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteCustomerAsync(int customerId)
        {
            var query = "UPDATE Customers SET IsDeleted = 1 WHERE CustomerId = @CustomerId";

            using (var connection = _connectionFactory.CreateConnection())
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CustomerId", customerId);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<CustomerResponseDTO> RegisterCustomerAsync(CustomerRegistrationDTO customer)
        {
            var query = @"INSERT INTO Customers (Name, Email, PasswordHash, Address, role, IsDeleted) 
                        VALUES (@Name, @Email, @PasswordHash, @Address, @role, 0);
                        SELECT CAST(SCOPE_IDENTITY() as int);";

            using (var connection = _connectionFactory.CreateConnection())
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", customer.Name);
                    command.Parameters.AddWithValue("@Email", customer.Email);
                    command.Parameters.AddWithValue("@PasswordHash", HashPassword(customer.Password));
                    command.Parameters.AddWithValue("@Address", customer.Address ?? string.Empty);
                    command.Parameters.AddWithValue("@role", customer.Role);

                    int customerId = (int)await command.ExecuteScalarAsync();
                    CustomerResponseDTO dto=new CustomerResponseDTO();
                    dto.CustomerId = customerId;
                    dto.Token = customer.Role;
                    return dto;
                }
            }
        }

        public async Task<CustomerResponseDTO?> LoginCustomerAsync(CustomerLoginDTO login)
        {
            var query = "SELECT CustomerId, PasswordHash,role FROM Customers WHERE Email = @Email AND IsDeleted = 0";

            using (var connection = _connectionFactory.CreateConnection())
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", login.Email);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var customerId = reader.GetInt32(reader.GetOrdinal("CustomerId"));
                            var storedPasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash"));
                            var role = reader.GetString(reader.GetOrdinal("role"));

                            if (VerifyPassword(login.Password, storedPasswordHash))
                            {
                                return new CustomerResponseDTO
                                {
                                    CustomerId = customerId,
                                    Token = role
                                };
                            }
                        }
                    }
                }
            }

            return null;
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            return HashPassword(password) == storedHash;
        }

        private string GenerateToken(int customerId)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{customerId}:{DateTime.UtcNow}"));
        }
    }
}
