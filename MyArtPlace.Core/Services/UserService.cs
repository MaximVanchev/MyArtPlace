using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyArtPlace.Areas.Identity.Data;
using MyArtPlace.Core.Contracts;
using MyArtPlace.Core.Models.User;
using MyArtPlace.Infrastructure.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyArtPlace.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IApplicationDbRepository repo;
        private readonly SignInManager<MyArtPlaceUser> signInManager;

        public UserService(IApplicationDbRepository _repo , SignInManager<MyArtPlaceUser> _signInManager)
        {
            repo = _repo;
            signInManager = _signInManager;
        }

        public async Task EditUser(ProfileEditViewModel model)
        {
            var user = await repo.GetByIdAsync<MyArtPlaceUser>(model.Id);

            if (model.ProfilePicture != null && model.ProfilePicture.Length > 0)
            {
                var imageName = "d" + DateTime.UtcNow.ToString()
                                        .Replace("/", "")
                                        .Replace(" ", "")
                                        .Replace(":", "")
                                        + model.ProfilePicture.FileName
                                        .ToLower();

                using (var stream = new FileStream($"wwwroot/corona_bootstrap/images/users_profile_pictures/{imageName}", FileMode.Create))
                {
                    await model.ProfilePicture.CopyToAsync(stream);
                }

                user.ProfilePicture = imageName;
            }

            user.UserName = model.Username;
            user.NormalizedUserName = model.Username.ToUpper();
            user.Email = model.Email;
            user.NormalizedEmail = model.Email.ToUpper();

            await signInManager.RefreshSignInAsync(user);

            await repo.SaveChangesAsync();
        }

        public async Task<MyArtPlaceUser> GetUserById(string id)
        {
            return await repo.GetByIdAsync<MyArtPlaceUser>(id);
        }

        public async Task<MyArtPlaceUser?> GetUserByUsername(string username)
        {
            return await repo.All<MyArtPlaceUser>().FirstOrDefaultAsync(u => u.UserName == username);
        }

        public async Task<ProfileEditViewModel> GetUserForEdit(string id)
        {
            var user = await repo.GetByIdAsync<MyArtPlaceUser>(id);

            var picturePath = user.ProfilePicture;

            if (picturePath == null)
            {
                picturePath = "../corona_bootstrap/images/users_profile_pictures/avatar_03.png";
            }
            else
            {
                picturePath = $"../corona_bootstrap/images/users_profile_pictures/{picturePath}";
            }

            return new ProfileEditViewModel
            {
                Id = id,
                Username = user.UserName,
                Email = user.Email,
                ImagePath = picturePath
            };
        }

        public async Task<IEnumerable<UserListViewModel>> GetUsers()
        {
            return await repo.All<MyArtPlaceUser>().Select(u => new UserListViewModel
            {
                Username = u.UserName,
                Email = u.Email,
                Id = u.Id
            }).ToListAsync();
        }
    }
}
