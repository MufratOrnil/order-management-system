using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace OrderManagement.Web.ViewModels
{
    public class OrderItemViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Product Name")]
        public string ProductName { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Display(Name = "Product Image")]
        public IFormFile? ImageFile { get; set; } // For uploading image

        public string? ImagePath { get; set; } // For displaying existing image
    }
}