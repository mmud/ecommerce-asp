public class ProductDTO
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string Description { get; set; }
    public string? ImagePath { get; set; } // Main image
    public string? AdditionalImage1 { get; set; } // Additional image 1
    public string? AdditionalImage2 { get; set; } // Additional image 2
    public string? AdditionalImage3 { get; set; } // Additional image 3
}