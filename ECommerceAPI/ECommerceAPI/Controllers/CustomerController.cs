using Microsoft.AspNetCore.Mvc;
using ECommerceAPI.Data;
using ECommerceAPI.DTO;
using ECommerceAPI.Models;
using System.Net;

namespace ECommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly CustomerRepository _customerRepository;

        public CustomerController(CustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        // GET: api/customer
        [HttpGet]
        public async Task<APIResponse<List<Customer>>> GetAllCustomers()
        {
            try
            {
                var customers = await _customerRepository.GetAllCustomersAsync();
                return new APIResponse<List<Customer>>(customers, "Retrieved all customers successfully.");
            }
            catch (Exception ex)
            {
                return new APIResponse<List<Customer>>(HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message);
            }
        }

        // GET: api/customer/5
        [HttpGet("{id}")]
        public async Task<APIResponse<Customer>> GetCustomerById(int id)
        {
            try
            {
                var customer = await _customerRepository.GetCustomerByIdAsync(id);
                if (customer == null)
                {
                    return new APIResponse<Customer>(HttpStatusCode.NotFound, "Customer not found.");
                }
                return new APIResponse<Customer>(customer, "Customer retrieved successfully.");
            }
            catch (Exception ex)
            {
                return new APIResponse<Customer>(HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message);
            }
        }

        // POST: api/customer/register
        [HttpPost("register")]
        public async Task<APIResponse<CustomerResponseDTO>> RegisterCustomer([FromBody] CustomerRegistrationDTO customerDto)
        {
            if (!ModelState.IsValid)
            {
                return new APIResponse<CustomerResponseDTO>(HttpStatusCode.BadRequest, "Invalid data", ModelState);
            }

            try
            {
                var customerrestdo = await _customerRepository.RegisterCustomerAsync(customerDto);
                
                return new APIResponse<CustomerResponseDTO>(customerrestdo, "Customer registered successfully.");
            }
            catch (Exception ex)
            {
                return new APIResponse<CustomerResponseDTO>(HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message);
            }
        }

        // POST: api/customer/login
        [HttpPost("login")]
        public async Task<APIResponse<CustomerResponseDTO>> LoginCustomer([FromBody] CustomerLoginDTO loginDto)
        {
            if (!ModelState.IsValid)
            {
                return new APIResponse<CustomerResponseDTO>(HttpStatusCode.BadRequest, "Invalid data", ModelState);
            }

            try
            {
                var response = await _customerRepository.LoginCustomerAsync(loginDto);
                if (response == null)
                {
                    return new APIResponse<CustomerResponseDTO>(HttpStatusCode.Unauthorized, "Invalid email or password.");
                }
                return new APIResponse<CustomerResponseDTO>(response, "Login successful.");
            }
            catch (Exception ex)
            {
                return new APIResponse<CustomerResponseDTO>(HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message);
            }
        }

        // POST: api/customer
        [HttpPost]
        public async Task<APIResponse<CustomerResponseDTO>> CreateCustomer([FromBody] CustomerDTO customerDto)
        {
            if (!ModelState.IsValid)
            {
                return new APIResponse<CustomerResponseDTO>(HttpStatusCode.BadRequest, "Invalid data", ModelState);
            }

            try
            {
                var customerId = await _customerRepository.InsertCustomerAsync(customerDto);
                var responseDTO = new CustomerResponseDTO { CustomerId = customerId };
                return new APIResponse<CustomerResponseDTO>(responseDTO, "Customer Created Successfully.");
            }
            catch (Exception ex)
            {
                return new APIResponse<CustomerResponseDTO>(HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message);
            }
        }

        // PUT: api/customer/5
        [HttpPut("{id}")]
        public async Task<APIResponse<bool>> UpdateCustomer(int id, [FromBody] CustomerDTO customerDto)
        {
            if (!ModelState.IsValid)
            {
                return new APIResponse<bool>(HttpStatusCode.BadRequest, "Invalid data", ModelState);
            }

            if (id != customerDto.CustomerId)
            {
                return new APIResponse<bool>(HttpStatusCode.BadRequest, "Mismatched Customer ID");
            }

            try
            {
                await _customerRepository.UpdateCustomerAsync(customerDto);
                return new APIResponse<bool>(true, "Customer Updated Successfully.");
            }
            catch (Exception ex)
            {
                return new APIResponse<bool>(HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message);
            }
        }

        // DELETE: api/customer/5
        [HttpDelete("{id}")]
        public async Task<APIResponse<bool>> DeleteCustomer(int id)
        {
            try
            {
                var customer = await _customerRepository.GetCustomerByIdAsync(id);
                if (customer == null)
                {
                    return new APIResponse<bool>(HttpStatusCode.NotFound, "Customer not found.");
                }

                await _customerRepository.DeleteCustomerAsync(id);
                return new APIResponse<bool>(true, "Customer deleted successfully.");
            }
            catch (Exception ex)
            {
                return new APIResponse<bool>(HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message);
            }
        }
    }
}