using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyArtPlace.Core.Models.Product
{
    public class ProductDetailsViewModel
    {
        public string Name { get; set; }

        public string? Description { get; set; }

        public byte[]? ImageByteArray { get; set; }

        public string Category { get; set; }

        public string Currency { get; set; }

        public Decimal Price { get; set; }
    }
}
