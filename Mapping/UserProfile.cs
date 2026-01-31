using System;
using apiEcommerce.Models;
using apiEcommerce.Models.Dtos;
using AutoMapper;

namespace apiEcommerce.Mapping;

public class UserProfile : Profile
{
  public UserProfile()
  {
    CreateMap<User, UserDto>().ReverseMap();
    CreateMap<UserDto, ApplicationUser>().ReverseMap();
    CreateMap<ApplicationUser, UserDataDto>().ReverseMap();
    CreateMap<User, CreateUserDto>().ReverseMap();
    CreateMap<User, UserRegisterDto>().ReverseMap();
    CreateMap<User, UserLoginDto>().ReverseMap();
    CreateMap<User, UserLoginResponseDto>().ReverseMap();
    CreateMap<User, UserUpdateDto>().ReverseMap();
    CreateMap<User, UserChangePasswordDto>().ReverseMap();
  }
}
