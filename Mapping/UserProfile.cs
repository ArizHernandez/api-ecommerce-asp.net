
using apiEcommerce.Models;
using apiEcommerce.Models.Dtos;
using Mapster;

namespace apiEcommerce.Mapping;

public static class UserProfile
{
    public static void Register(TypeAdapterConfig config)
    {
        config.NewConfig<User, UserDto>().TwoWays();
        config.NewConfig<UserDto, ApplicationUser>().TwoWays();
        config.NewConfig<ApplicationUser, UserDataDto>().TwoWays();
        config.NewConfig<User, CreateUserDto>().TwoWays();
        config.NewConfig<User, UserRegisterDto>().TwoWays();
        config.NewConfig<User, UserLoginDto>().TwoWays();
        config.NewConfig<User, UserLoginResponseDto>().TwoWays();
        config.NewConfig<User, UserUpdateDto>().TwoWays();
        config.NewConfig<User, UserChangePasswordDto>().TwoWays();
    }
}
