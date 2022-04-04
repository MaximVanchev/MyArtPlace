using MyArtPlace.Infrastructure.Data;
using MyArtPlace.Infrastructure.Data.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyArtPlace.Core.Models.Shop
{
    public class ShopViewModel
    {
        public ShopViewModel()
        {
            AllCurrencies = new List<Currency>();
        }

        [Required]
        [StringLength(DatabaseConstants.Shop_Name_Max_Length, MinimumLength = DatabaseConstants.Name_Min_Length)]
        public string Name { get; set; }

        [StringLength(DatabaseConstants.Description_Max_Length)]
        public string? Description { get; set; }

        [StringLength(DatabaseConstants.Location_Max_Length)]
        public string? Location { get; set; }

        [Required]
        [StringLength(DatabaseConstants.Currency_Iso_Length, MinimumLength = DatabaseConstants.Currency_Iso_Length)]
        public string Currency { get; set; }

        public IEnumerable<Currency> AllCurrencies { get; set; }
    }
}
