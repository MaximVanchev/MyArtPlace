using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyArtPlace.Test
{
    public class FakeUserManager<TUser> : UserManager<TUser>
        where TUser : class
    {
        public FakeUserManager()
            : base(new Mock<IUserStore<TUser>>().Object,
              new Mock<IOptions<IdentityOptions>>().Object,
              new Mock<IPasswordHasher<TUser>>().Object,
              new IUserValidator<TUser>[0],
              new IPasswordValidator<TUser>[0],
              new Mock<ILookupNormalizer>().Object,
              new Mock<IdentityErrorDescriber>().Object,
              new Mock<IServiceProvider>().Object,
              new Mock<ILogger<UserManager<TUser>>>().Object)
        { }

        public override Task<IdentityResult> CreateAsync(TUser user, string password)
        {
            return Task.FromResult(IdentityResult.Success);
        }

        public override Task<IdentityResult> AddToRoleAsync(TUser user, string role)
        {
            return Task.FromResult(IdentityResult.Success);
        }

        public override Task<string> GenerateEmailConfirmationTokenAsync(TUser user)
        {
            return Task.FromResult(Guid.NewGuid().ToString());
        }

    }
}
