using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyArtPlace.Core.Models.User
{
    public class ProfileEditViewModel
    {
        public string Username { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public byte[]? ProfilePicture { get; set; }
    }
}
