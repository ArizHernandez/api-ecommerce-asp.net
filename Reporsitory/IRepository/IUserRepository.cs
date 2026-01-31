using System;
using apiEcommerce.Models;
using apiEcommerce.Models.Dtos;

namespace apiEcommerce.Reporsitory.IRepository;

public interface IUserRepository
{
  public ICollection<ApplicationUser> GetUsers();
  public ApplicationUser? GetUser(string id);
  public bool IsUniqueUser(string userName);
  public Task<UserLoginResponseDto> Login(UserLoginDto userLoginDto);
  public Task<UserDataDto> Register(CreateUserDto createUserDto);
  public bool ChangeUserPassword(User user);
  public bool UpdateUser(User user);
  public bool Save();
}
