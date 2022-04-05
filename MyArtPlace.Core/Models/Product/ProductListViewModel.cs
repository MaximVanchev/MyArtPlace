using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyArtPlace.Core.Models.Product
{
    public class ProductListViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public byte[] ImageByteArray { get; set; }

        public string Category { get; set; }

        public int Likes { get; set; }

        public bool UserLiked { get; set; }

        public Decimal Price { get; set; }

        public string Iso { get; set; }
    }
}
