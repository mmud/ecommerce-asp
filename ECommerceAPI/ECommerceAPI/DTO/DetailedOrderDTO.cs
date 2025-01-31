namespace ECommerceAPI.DTO
{
    public class DetailedOrderDTO
    {
        public int OrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public DateTime OrderDate { get; set; }
        public CustomerDTO Customer { get; set; }
        public List<DetailedOrderItemDTO> Items { get; set; }
    }

}
