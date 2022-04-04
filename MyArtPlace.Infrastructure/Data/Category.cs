using MyArtPlace.Infrastructure.Data.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyArtPlace.Infrastructure.Data
{
    public class Category
    {
        public Category()
        {
            Products = new List<Product>();
            Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(DatabaseConstants.Category_Name_Max_Length)]
        public string Name { get; set; }

        public IList<Product> Products { get; set; }
    }
}
