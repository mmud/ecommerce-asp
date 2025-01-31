using Microsoft.AspNetCore.Mvc;
using ECommerceAPI.Data;
using ECommerceAPI.DTO;
using ECommerceAPI.Models;
using System.Net;

namespace ECommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly OrderRepository _orderRepository;

        public OrderController(OrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        // GET: api/order
        [HttpGet]
        public async Task<APIResponse<List<DetailedOrderDTO>>> GetAllOrders()
        {
            try
            {
                var orders = await _orderRepository.GetAllOrdersAsync();
                return new APIResponse<List<DetailedOrderDTO>>(orders, "Retrieved all orders successfully.");
            }
            catch (Exception ex)
            {
                return new APIResponse<List<DetailedOrderDTO>>(HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message);
            }
        }


        // POST: api/order
        [HttpPost]
        public async Task<APIResponse<CreateOrderResponseDTO>> CreateOrder([FromBody] OrderDTO orderDto)
        {
            if (!ModelState.IsValid)
            {
                return new APIResponse<CreateOrderResponseDTO>(HttpStatusCode.BadRequest, "Invalid data", ModelState);
            }

            try
            {
                var response = await _orderRepository.CreateOrderAsync(orderDto);
                return new APIResponse<CreateOrderResponseDTO>(response, response.Message);
            }
            catch (Exception ex)
            {
                return new APIResponse<CreateOrderResponseDTO>(HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message);
            }
        }

        // DELETE: api/order/5
        [HttpDelete("{id}")]
        public async Task<APIResponse<bool>> DeleteOrder(int id)
        {
            try
            {
                var isDeleted = await _orderRepository.DeleteOrderAsync(id);

                if (isDeleted)
                {
                    return new APIResponse<bool>(true, "Order deleted successfully.");
                }
                else
                {
                    return new APIResponse<bool>(HttpStatusCode.NotFound, "Order not found or could not be deleted.");
                }
            }
            catch (Exception ex)
            {
                return new APIResponse<bool>(HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message);
            }
        }

    }
}