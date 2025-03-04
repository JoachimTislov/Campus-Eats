using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Helpers;

public class FakeRoleManager : RoleManager<IdentityRole<Guid>>
{
    public FakeRoleManager()
            : base
            (new Mock<IRoleStore<IdentityRole<Guid>>>().Object,
            [],
            new Mock<ILookupNormalizer>().Object,
            new Mock<IdentityErrorDescriber>().Object,
            new Mock<ILogger<RoleManager<IdentityRole<Guid>>>>().Object)
    { }
}
