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
        private readonly ApplicationDbRepository repo;

        public UserService(ApplicationDbRepository _repo)
        {
            repo = _repo;
        }

        public async Task<MyArtPlaceUser> GetUserById(string id)
        {
            return await repo.GetByIdAsync<MyArtPlaceUser>(id);
        }

        public Task<ProfileEditViewModel> GetUserForEdit(string id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<MyArtPlaceUser> GetUsers()
        {
            return repo.All<MyArtPlaceUser>();
        }
    }
}
