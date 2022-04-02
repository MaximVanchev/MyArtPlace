using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyArtPlace.Core.Models.User
{
    public class UserListViewModel
    {
        public string Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }
    }
}
