using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyArtPlace.Test
{
    public class FakeRoleManager<TRole> : RoleManager<TRole>
        where TRole : class
    {
        public FakeRoleManager()
            : base(new Mock<IQueryableRoleStore<TRole>>().Object,
                 new IRoleValidator<TRole>[0],
                 new Mock<ILookupNormalizer>().Object,
                 new Mock<IdentityErrorDescriber>().Object,
                 new Mock<ILogger<RoleManager<TRole>>>().Object)
        { }
    }
}
