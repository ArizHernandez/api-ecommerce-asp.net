using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using apiEcommerce.Data;
using apiEcommerce.Models;
using apiEcommerce.Models.Dtos;
using apiEcommerce.Reporsitory.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace apiEcommerce.Reporsitory;

public class UserRepository : IUserRepository
{
  private readonly ApplicationDBContext _db;
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly RoleManager<IdentityRole> _roleManager;
  private readonly IMapper _mapper;
  private string? secretKey;

  public UserRepository(ApplicationDBContext db, IConfiguration configuration,
                        UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper)
  {
    _db = db;
    secretKey = configuration.GetValue<string>("ApiSettings:SecretJWT");
    _userManager = userManager;
    _roleManager = roleManager;
    _mapper = mapper;
  }

  public bool ChangeUserPassword(User user)
  {
    throw new NotImplementedException();
  }

  public ApplicationUser? GetUser(string id)
  {
    return _db.ApplicationUsers.FirstOrDefault(u => u.Id == id);
  }

  public ICollection<ApplicationUser> GetUsers()
  {
    return _db.ApplicationUsers.OrderBy((u) => u.UserName).ToList();
  }

  public bool IsUniqueUser(string userName)
  {
    return !_db.ApplicationUsers.Any(u => !string.IsNullOrEmpty(u.UserName) && u.UserName.ToLower().Trim() == userName.ToLower().Trim());
  }

  public async Task<UserLoginResponseDto> Login(UserLoginDto userLoginDto)
  {
    if (string.IsNullOrEmpty(userLoginDto.UserName))
    {
      return new UserLoginResponseDto()
      {
        Token = "",
        User = null,
        Message = "username is required"
      };
    }

    var user = await _db.ApplicationUsers.FirstOrDefaultAsync(u => u.UserName != null && u.UserName.ToLower().Trim() == userLoginDto.UserName.ToLower().Trim());
    if (user == null)
    {
      return new UserLoginResponseDto()
      {
        Token = "",
        User = null,
        Message = "Invalid credentials"
      };
    }

    if (userLoginDto.Password == null)
    {
      return new UserLoginResponseDto()
      {
        Token = "",
        User = null,
        Message = "Invalid credentials"
      };
    }
    bool isValid = await _userManager.CheckPasswordAsync(user, userLoginDto.Password);
    if (!isValid)
    {
      return new UserLoginResponseDto()
      {
        Token = "",
        User = null,
        Message = "Invalid credentials"
      };
    }

    // if (!BCrypt.Net.BCrypt.Verify(userLoginDto.Password, user.Password))
    // {
    //   return new UserLoginResponseDto()
    //   {
    //     Token = "",
    //     User = null,
    //     Message = "Invalid credentials"
    //   };
    // }

    var handlerToken = new JwtSecurityTokenHandler();
    if (string.IsNullOrWhiteSpace(secretKey))
    {
      throw new InvalidOperationException("Secret key not configurated");
    }

    var roles = await _userManager.GetRolesAsync(user);
    var key = Encoding.UTF8.GetBytes(secretKey);
    var tokenDescriptor = new SecurityTokenDescriptor
    {
      Subject = new ClaimsIdentity(new[]{
        new Claim("id", user.Id.ToString()),
        new Claim("username", user.UserName ?? string.Empty),
        new Claim(ClaimTypes.Role, roles.FirstOrDefault() ?? string.Empty)
      }),
      Expires = DateTime.UtcNow.AddHours(2),
      SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    };
    var token = handlerToken.CreateToken(tokenDescriptor);

    return new UserLoginResponseDto
    {
      Token = handlerToken.WriteToken(token),
      User = _mapper.Map<UserDataDto>(user),
      Message = "User logged succesfully!"
    };
  }

  // public async Task<User> Register(CreateUserDto createUserDto)
  // {
  //   var passwordEncrypted = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password);
  //   var user = new User
  //   {
  //     Name = createUserDto.Name,
  //     Role = createUserDto.Role,
  //     UserName = createUserDto.UserName,
  //     Password = passwordEncrypted,
  //   };

  //   _db.Users.Add(user);
  //   await _db.SaveChangesAsync();
  //   return user;
  // }

  public async Task<UserDataDto> Register(CreateUserDto createUserDto)
  {
    if (string.IsNullOrEmpty(createUserDto.UserName))
    {
      throw new ArgumentNullException("UserName is required");
    }
    if (string.IsNullOrEmpty(createUserDto.Password))
    {
      throw new ArgumentNullException("Invalid credentials");
    }

    var user = new ApplicationUser
    {
      UserName = createUserDto.UserName,
      Email = createUserDto.UserName,
      NormalizedEmail = createUserDto.UserName.ToUpper(),
      Name = createUserDto.Name,
    };
    var result = await _userManager.CreateAsync(user, createUserDto.Password);
    if (!result.Succeeded)
    {
      var errors = string.Join(",\n", result.Errors.Select(e => e.Description));
      throw new Exception($"Couldn't create user: {errors}");
    }

    var userRole = createUserDto.Role ?? "User";
    var roleExists = await _roleManager.RoleExistsAsync(userRole);
    if (!roleExists)
    {
      var identityRole = new IdentityRole(userRole);
      await _roleManager.CreateAsync(identityRole);
    }

    await _userManager.AddToRoleAsync(user, userRole);
    var createdUser = _db.ApplicationUsers.FirstOrDefault(u => u.UserName == createUserDto.UserName);
    return _mapper.Map<UserDataDto>(createdUser);
  }

  public bool UpdateUser(User user)
  {
    _db.Users.Update(user);
    return Save();
  }

  public bool Save()
  {
    return _db.SaveChanges() > 0;
  }
}
