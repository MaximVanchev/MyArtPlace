using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyArtPlace.Core.Models.Cart
{
    public class CartViewModel
    {
        public Guid CartId { get; set; }

        public Guid ProductId { get; set; }

        public string ProductName { get; set; }

        public Decimal ProductPrice { get; set; }

        public string ProductIso { get; set; }

        public int ProductCount { get; set; }
    }
}
