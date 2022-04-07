using MyArtPlace.Infrastructure.Data;
using MyArtPlace.Infrastructure.Data.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyArtPlace.Core.Models.Cart
{
    public class CartListViewModel
    {
        public CartListViewModel()
        {
            AllCurrencies = new List<Currency>();
            CartProducts = new List<CartViewModel>();
        }

        public string? Currency { get; set; }

        public IEnumerable<CartViewModel>? CartProducts { get; set; }

        public IEnumerable<Currency>? AllCurrencies { get; set; }
    }
}
