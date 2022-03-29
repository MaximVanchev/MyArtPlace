using MyArtPlace.Areas.Identity.Data;
using MyArtPlace.Core.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyArtPlace.Core.Contracts
{
    public interface IUserService
    {
        Task<MyArtPlaceUser> GetUserById(string id);

        IQueryable<MyArtPlaceUser> GetUsers();

        Task<ProfileEditViewModel> GetUserForEdit(string id);
    }
}
