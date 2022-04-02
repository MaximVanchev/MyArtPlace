using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyArtPlace.Areas.Identity.Data;
using MyArtPlace.Core.Constants;
using MyArtPlace.Core.Contracts;
using MyArtPlace.Core.Models.User;
using System.Security.Claims;

namespace MyArtPlace.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly SignInManager<MyArtPlaceUser> signInManager;
        private readonly IUserService userService;

        public UserController(IUserService _userService)
        {
            userService = _userService;
        }

        public async Task<IActionResult> Settings()
        {
            var user = await userService.GetUserForEdit(User.FindFirstValue(ClaimTypes.NameIdentifier));

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Settings(ProfileEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await userService.EditUser(model);
                ViewData[MessageConstants.SuccessMessage] = "Successful saved changes!";
            }
            catch (ArgumentException aex)
            {
                ViewData[MessageConstants.ErrorMessage] = aex.Message;
            }
            catch (Exception)
            {
                ViewData[MessageConstants.ErrorMessage] = "There was an error!";
            }

            return Redirect("/");
        }
    }
}
