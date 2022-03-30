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

        public UserService(IApplicationDbRepository _repo)
        {
            repo = _repo;
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

            return new ProfileEditViewModel
            {
                Id = id,
                Username = user.UserName,
                Email = user.Email,
                ProfilePicture = user.ProfilePicture
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
