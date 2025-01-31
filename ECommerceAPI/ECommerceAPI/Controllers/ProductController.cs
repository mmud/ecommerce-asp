using Microsoft.AspNetCore.Mvc;
using ECommerceAPI.Data;
using ECommerceAPI.DTO;
using ECommerceAPI.Models;
using System.Net;
using System.IO;
using Microsoft.Data.SqlClient;

namespace ECommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ProductRepository _productRepository;

        public ProductController(ProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        
        // GET: api/product
        [HttpGet]
        public async Task<APIResponse<List<Product>>> GetAllProducts()
        {
            try
            {
                var products = await _productRepository.GetAllProductsAsync();
                return new APIResponse<List<Product>>(products, "Retrieved all products successfully.");
            }
            catch (Exception ex)
            {
                return new APIResponse<List<Product>>(HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message);
            }
        }

        // GET: api/product/5
        [HttpGet("{id}")]
        public async Task<APIResponse<Product>> GetProductById(int id)
        {
            try
            {
                var product = await _productRepository.GetProductByIdAsync(id);
                if (product == null)
                {
                    return new APIResponse<Product>(HttpStatusCode.NotFound, "Product not found.");
                }
                return new APIResponse<Product>(product, "Product retrieved successfully.");
            }
            catch (Exception ex)
            {
                return new APIResponse<Product>(HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message);
            }
        }

        [HttpPost]
        public async Task<APIResponse<ProductResponseDTO>> CreateProduct([FromForm] ProductFormDTO form)
        {
            ProductDTO product=new ProductDTO();
            product.Name = form.Name;
            product.Price = form.Price;
            product.Description = form.Description;
            product.Quantity = form.Quantity;
            if (form.MainImage != null)
                product.ImagePath = await SaveImageAsync(form.MainImage);

            if (form.AdditionalImage1 != null)
                product.AdditionalImage1 = await SaveImageAsync(form.AdditionalImage1);

            if (form.AdditionalImage2 != null)
                product.AdditionalImage2 = await SaveImageAsync(form.AdditionalImage2);

            if (form.AdditionalImage3 != null)
                product.AdditionalImage3 = await SaveImageAsync(form.AdditionalImage3);

            var productId = await _productRepository.InsertProductAsync(product);
            return new APIResponse<ProductResponseDTO>(new ProductResponseDTO { ProductId = productId }, "Product created successfully.");
        }
        
        [HttpPut("{id}")]
        public async Task<APIResponse<bool>> UpdateProduct(int id, [FromForm] ProductFormDTO form)
        {
            ProductDTO product = new ProductDTO();
            product.ProductId = id;
            product.Name = form.Name;
            product.Price = form.Price;
            product.Description = form.Description;
            product.Quantity = form.Quantity;
            await _productRepository.UpdateProductAsync(product);
            return new APIResponse<bool>(true, "Product updated successfully.");
        }
        


        // DELETE: api/product/5
        [HttpDelete("{id}")]
        public async Task<APIResponse<bool>> DeleteProduct(int id)
        {
            try
            {
                var product = await _productRepository.GetProductByIdAsync(id);
                if (product == null)
                {
                    return new APIResponse<bool>(HttpStatusCode.NotFound, "Product not found.");
                }

                await _productRepository.DeleteProductAsync(id);
                return new APIResponse<bool>(true, "Product deleted successfully.");
            }
            catch (Exception ex)
            {
                return new APIResponse<bool>(HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message);
            }
        }
        private readonly string ImageFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

        private async Task<string> SaveImageAsync(IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                throw new ArgumentException("Invalid image file.");
            }

            if (!Directory.Exists(ImageFolderPath))
            {
                Directory.CreateDirectory(ImageFolderPath);
            }

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(image.FileName)}";
            var filePath = Path.Combine(ImageFolderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            return $"/images/{fileName}";
        }

    }
}
