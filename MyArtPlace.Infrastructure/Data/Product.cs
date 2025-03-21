﻿using MyArtPlace.Areas.Identity.Data;
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
    public class Product
    {
        public Product()
        {
            UsersLiked = new List<MyArtPlaceUser>();
            UsersCarts = new List<Cart>();
        }

        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(DatabaseConstants.Product_Name_Max_Length, MinimumLength = DatabaseConstants.Name_Min_Length)]
        public string Name { get; set; }

        [StringLength(DatabaseConstants.Description_Max_Length)]
        public string? Description { get; set; }

        [Required]
        public byte[] Image { get; set; }

        [Required]
        [Range(DatabaseConstants.Price_Min_Range ,DatabaseConstants.Price_Max_Pange)]
        [DataType("decimal(19,4)")]
        public Decimal Price { get; set; }

        public IList<MyArtPlaceUser> UsersLiked { get; set; }

        public IList<Cart> UsersCarts { get; set; }

        [Required]
        [StringLength(DatabaseConstants.Guid_Max_Length)]
        public Guid ShopId { get; set; }

        [ForeignKey(nameof(ShopId))]
        public Shop Shop { get; set; }

        [Required]
        [StringLength(DatabaseConstants.Guid_Max_Length)]
        public Guid CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; }
    }
}
