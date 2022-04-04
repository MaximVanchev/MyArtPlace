using MyArtPlace.Infrastructure.Data.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyArtPlace.Infrastructure.Data
{
    public class Currency
    {
        public Currency()
        {
            Shops = new List<Shop>();
            Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(DatabaseConstants.Currency_Iso_Length)]
        public string Iso { get; set; }

        public List<Shop> Shops { get; set; }
    }
}
