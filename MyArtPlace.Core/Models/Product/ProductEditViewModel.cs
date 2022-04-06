using Microsoft.AspNetCore.Http;
using MyArtPlace.Infrastructure.Data;
using MyArtPlace.Infrastructure.Data.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyArtPlace.Core.Models.Product
{
    public class ProductEditViewModel
    {
        public ProductEditViewModel()
        {
            AllCategories = new List<Category>();
        }
        [Required]
        public Guid Id { get; set; }

        [Required]
        [StringLength(DatabaseConstants.Product_Name_Max_Length, MinimumLength = DatabaseConstants.Name_Min_Length)]
        public string Name { get; set; }

        [StringLength(DatabaseConstants.Description_Max_Length)]
        public string? Description { get; set; }

        public byte[]? ImageByteArray { get; set; }

        public IFormFile? Image { get; set; }

        [Required]
        public string Category { get; set; }

        public IEnumerable<Category> AllCategories { get; set; }

        [Required]
        [Range(DatabaseConstants.Price_Min_Range, DatabaseConstants.Price_Max_Pange)]
        public Decimal Price { get; set; }
    }
}
