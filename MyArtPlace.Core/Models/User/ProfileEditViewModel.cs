using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyArtPlace.Core.Models.User
{
    public class ProfileEditViewModel
    {
        public string Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Email { get; set; }

        public IFormFile? ProfilePicture { get; set; }

        public string? ImagePath { get; set; }
    }
}
