using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyArtPlace.Areas.Identity.Data;
using MyArtPlace.Core.Constants;
using MyArtPlace.Core.Contracts;
using MyArtPlace.Core.Models.Common;
using MyArtPlace.Core.Models.User;
using System.Security.Claims;

namespace MyArtPlace.Controllers
{
    [Authorize]
    public class UserController : BaseController
    {
        private readonly IUserService userService;

        public UserController(IUserService _userService)
        {
            userService = _userService;
        }

        public async Task<IActionResult> Settings()
        {
            try
            {
                await CheckMessages();

                var user = await userService.GetUserForEdit(User.FindFirstValue(ClaimTypes.NameIdentifier));

                return View(user);
            }
            catch (Exception)
            {
                MessageViewModel.Message.Add(MessageConstants.ErrorMessage, MessageConstants.ThereWasErrorMessage);
                return Redirect("/");
            }
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
                MessageViewModel.Message.Add(MessageConstants.SuccessMessage, MessageConstants.SuccessfulSavedChanges);
            }
            catch (Exception)
            {
                MessageViewModel.Message.Add(MessageConstants.ErrorMessage, MessageConstants.ThereWasErrorMessage);
            }

            return Redirect("/");
        }
    }
}
