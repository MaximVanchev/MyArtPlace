using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyArtPlace.Areas.Identity.Data;
using MyArtPlace.Core.Constants;
using MyArtPlace.Core.Contracts;
using MyArtPlace.Core.Models.Admin;
using MyArtPlace.Core.Models.Common;

namespace MyArtPlace.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : BaseController
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<MyArtPlaceUser> userManager;
        private readonly IUserService userService;

        public AdminController(RoleManager<IdentityRole> _roleManager, UserManager<MyArtPlaceUser> _userManager, IUserService _userService)
        {
            roleManager = _roleManager;
            userManager = _userManager;
            userService = _userService;
        }

        public async Task<IActionResult> CreateRole()
        {
            await CheckMessages();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await roleManager.CreateAsync(new IdentityRole
            {
                Name = model.Name
            });

            MessageViewModel.Message.Add(MessageConstants.SuccessMessage, MessageConstants.SuccessfulCreatedRoleMessage);

            return Redirect("/");
        }

        public async Task<IActionResult> ManageUsers()
        {
            await CheckMessages();

            var users = await userService.GetUsers();

            return View(users);
        }

        public async Task<IActionResult> Roles(string id)
        {
            await CheckMessages();

            var user = await userService.GetUserById(id);
            var model = new UserRolesViewModel
            {
                Id = user.Id,
                Username = user.UserName
            };

            ViewBag.RoleItem = roleManager.Roles
                .ToList()
                .Select(async r => new SelectListItem
                {
                    Text = r.Name,
                    Value = r.Name,
                    Selected = userManager.IsInRoleAsync(user, r.Name).Result
                }).ToList();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Roles(UserRolesViewModel model)
        {
            var user = await userService.GetUserById(model.Id);
            var userRoles = await userManager.GetRolesAsync(user);

            await userManager.RemoveFromRolesAsync(user, userRoles);

            if (model.RoleNames?.Length > 0)
            {
                await userManager.AddToRolesAsync(user, model.RoleNames);
            }

            MessageViewModel.Message.Add(MessageConstants.SuccessMessage, MessageConstants.SuccessfulSavedChanges);

            return RedirectToAction(nameof(ManageUsers));
        }

        //public async Task<IActionResult> Search(string Username)
        //{
        //    var user = await userService.GetUserByUsername(Username);
        //    var users = new List<MyArtPlaceUser>();

        //    if (user != null)
        //    {
        //        users.Add(user);
        //    }

        //    return View("ManageUsers", users);
        //}
    }
}
