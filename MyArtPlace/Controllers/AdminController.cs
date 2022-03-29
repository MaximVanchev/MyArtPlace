using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyArtPlace.Core.Services;

namespace MyArtPlace.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<IdentityUser> userManager;
        private readonly UserService userService;

        public AdminController(RoleManager<IdentityRole> _roleManager , UserManager<IdentityUser> _userManager)
        {
            roleManager = _roleManager;
            userManager = _userManager;
        }

        public async Task<IActionResult> CreateRole()
        {
            await roleManager.CreateAsync(new IdentityRole
            {
                Name = "Admin"
            });

            return Ok();
        }

        public async Task<IActionResult> ManageUsers()
        {
            var users = userService.GetUsers();

            return View(users);
        }

        public async Task<IActionResult> Roles()
        {
            return View();
        }
    }
}
