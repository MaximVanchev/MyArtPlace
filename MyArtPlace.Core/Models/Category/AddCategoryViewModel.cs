using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyArtPlace.Core.Models.Admin
{
    public class AddCategoryViewModel
    {
        [Required]
        public string Name { get; set; }
    }
}
