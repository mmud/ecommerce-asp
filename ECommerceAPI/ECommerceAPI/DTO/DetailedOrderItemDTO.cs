namespace ECommerceAPI.DTO
{
    public class DetailedOrderItemDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public decimal PriceAtOrder { get; set; }
        public int Quantity { get; set; }
    }
}
