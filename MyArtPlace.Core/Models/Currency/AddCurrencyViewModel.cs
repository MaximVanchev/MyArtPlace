using MyArtPlace.Infrastructure.Data.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyArtPlace.Core.Models.Admin
{
    public class AddCurrencyViewModel
    {
        [StringLength(DatabaseConstants.Currency_Iso_Length)]
        public string Iso { get; set; }
    }
}
