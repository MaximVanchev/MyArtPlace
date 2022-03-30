using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyArtPlace.Core.Models.Admin
{
    public class UserRolesViewModel
    {
        public string Id { get; set; }

        public string Username { get; set; }

        public string[] RoleNames { get; set; }
    }
}
