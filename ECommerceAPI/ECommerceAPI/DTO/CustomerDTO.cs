using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.DTO
{
    public class CustomerDTO
    {
        public int CustomerId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string Address { get; set; }
        [Required]
        public string Role { get; set; }
    }

    public class CustomerRegistrationDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; } 
        [Required]
        public string Role { get; set; }
        public string Address { get; set; }
    }

    public class CustomerLoginDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}