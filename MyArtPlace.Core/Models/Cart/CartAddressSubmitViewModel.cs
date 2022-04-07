using MyArtPlace.Infrastructure.Data.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyArtPlace.Core.Models.Cart
{
    public class CartAddressSubmitViewModel
    {
        [Required]
        [StringLength(DatabaseConstants.Oredr_Address_Max_Length)]
        public string? OrederAddress { get; set; }

        public string? Currency { get; set; }

        public decimal TotalPrice { get; set; }
    }
}
