using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
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
    public class FakeSignInManager<TUser> : SignInManager<TUser>
        where TUser : class
    {
        public FakeSignInManager()
                : base(new FakeUserManager<TUser>(),
                     new Mock<IHttpContextAccessor>().Object,
                     new Mock<IUserClaimsPrincipalFactory<TUser>>().Object,
                     new Mock<IOptions<IdentityOptions>>().Object,
                     new Mock<ILogger<SignInManager<TUser>>>().Object,
                     new Mock<IAuthenticationSchemeProvider>().Object,
                     new Mock<IUserConfirmation<TUser>>().Object)
        { }

        public override Task<IdentityResult> SignOutAsync()
        {
            return Task.FromResult(IdentityResult.Success);
        }
    }
}
