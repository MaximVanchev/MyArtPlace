using MyArtPlace.Areas.Identity.Data;
using MyArtPlace.Infrastructure.Data.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyArtPlace.Infrastructure.Data
{
    public class Shop
    {
        public Shop()
        {
            Id = Guid.NewGuid();
            Products = new List<Product>();
        }

        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(DatabaseConstants.Shop_Name_Max_Length , MinimumLength = DatabaseConstants.Name_Min_Length)]
        public string Name { get; set; }

        [StringLength(DatabaseConstants.Description_Max_Length)]
        public string? Description { get; set; }

        [StringLength(DatabaseConstants.Location_Max_Length)]
        public string? Location { get; set; }

        [Required]
        public MyArtPlaceUser User { get; set; }

        [Required]
        [StringLength(DatabaseConstants.Guid_Max_Length)]
        public Guid CurrencyId { get; set; }

        [ForeignKey(nameof(CurrencyId))]
        public Currency Currency { get; set; }

        public IList<Product> Products { get; set; }
    }
}
