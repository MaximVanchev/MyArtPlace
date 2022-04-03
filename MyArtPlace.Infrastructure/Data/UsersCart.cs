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
    public class UsersCart
    {
        public UsersCart()
        {
            OrderId = Guid.NewGuid();
            InCart = true;
        }

        [Key]
        public Guid OrderId { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public MyArtPlaceUser User { get; set; }

        [Required]
        public Guid ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }

        [Required]
        [Range(DatabaseConstants.Products_Count_Min_Length, DatabaseConstants.Products_Count_Max_Length)]
        public int ProductConut { get; set; }

        [Required]
        public bool InCart { get; set; }
    }
}
