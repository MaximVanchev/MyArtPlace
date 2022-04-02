using MyArtPlace.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyArtPlace.Infrastructure.Data
{
    public class UsersCart
    {
        [Required]
        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public MyArtPlaceUser User { get; set; }

        [Required]
        public Guid ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }

        [Required]
        [Range(1,10)]
        public int ProductConut { get; set; }
    }
}
