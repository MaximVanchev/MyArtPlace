using Microsoft.AspNetCore.Http;
using MyArtPlace.Infrastructure.Data.Constants;
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
        [StringLength(DatabaseConstants.Username_Max_Length, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long."
            , MinimumLength = DatabaseConstants.Name_Min_Length)]
        [Display(Name = "Username")]
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        public IFormFile? ProfilePicture { get; set; }

        [StringLength(DatabaseConstants.ProfilePicture_Max_Length)]
        public string? ImagePath { get; set; }
    }
}
