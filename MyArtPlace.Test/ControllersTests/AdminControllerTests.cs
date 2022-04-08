using Microsoft.AspNetCore.Identity;
using MyArtPlace.Areas.Identity.Data;
using MyArtPlace.Controllers;
using MyArtPlace.Core.Contracts;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyArtPlace.Test.ControllersTests
{
    public class AdminControllerTests
    {
        private readonly AdminController adminController;

        public AdminControllerTests(AdminController _adminController)
        {
            adminController = _adminController;
        }

        [SetUp]
        public async Task Setup()
        {

        }
    }
}
