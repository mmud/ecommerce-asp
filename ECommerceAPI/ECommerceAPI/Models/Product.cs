public class Product
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string Description { get; set; }
    public string? ImagePath { get; set; }
    public string? AdditionalImage1 { get; set; }
    public string? AdditionalImage2 { get; set; }
    public string? AdditionalImage3 { get; set; }
    public bool IsDeleted { get; set; }
}
