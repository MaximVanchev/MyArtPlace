﻿using MyArtPlace.Infrastructure.Data;
using MyArtPlace.Infrastructure.Data.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyArtPlace.Core.Models.Shop
{
    public class ShopEditViewModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [StringLength(DatabaseConstants.Shop_Name_Max_Length, MinimumLength = DatabaseConstants.Name_Min_Length)]
        public string Name { get; set; }

        [StringLength(DatabaseConstants.Description_Max_Length)]
        public string? Description { get; set; }

        [StringLength(DatabaseConstants.Location_Max_Length)]
        public string? Location { get; set; }

        [Required]
        [StringLength(DatabaseConstants.Guid_Max_Length)]
        public Guid CurrencyId { get; set; }

        [Required]
        public Currency Currency { get; set; }

        public IEnumerable<Currency> AllCurrencies { get; set; }
    }
}
