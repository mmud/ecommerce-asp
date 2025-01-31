public class ProductFormDTO
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string Description { get; set; }
    public IFormFile? MainImage { get; set; }
    public IFormFile? AdditionalImage1 { get; set; } 
    public IFormFile? AdditionalImage2 { get; set; }
    public IFormFile? AdditionalImage3 { get; set; } 
}
