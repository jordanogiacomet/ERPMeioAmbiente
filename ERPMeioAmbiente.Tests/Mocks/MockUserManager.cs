using Microsoft.AspNetCore.Identity;
using Moq;

public static class MockUserManager
{
    public static Mock<UserManager<TUser>> CreateMockUserManager<TUser>() where TUser : class
    {
        var store = new Mock<IUserStore<TUser>>();
        var mgr = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);

        mgr.Object.UserValidators.Add(new UserValidator<TUser>());
        mgr.Object.PasswordValidators.Add(new PasswordValidator<TUser>());

        mgr.Setup(x => x.CreateAsync(It.IsAny<TUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        mgr.Setup(x => x.AddToRoleAsync(It.IsAny<TUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        return mgr;
    }
}
