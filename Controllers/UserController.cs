using apiEcommerce.Constants;
using apiEcommerce.Models.Dtos;
using apiEcommerce.Reporsitory.IRepository;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace apiEcommerce.Controllers
{
  [Route("api/v{version:apiVersion}/[controller]")]
  [ApiVersionNeutral]
  [ApiController]
  [Authorize(Roles = "Admin")]
  [EnableCors(PolicyNames.AllowSpecificOrigin)]
  public class UserController : ControllerBase
  {
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserController(IUserRepository userRepository, IMapper mapper)
    {
      _userRepository = userRepository;
      _mapper = mapper;
    }

    [HttpGet(Name = "GetUsers")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetUsers()
    {
      var users = _userRepository.GetUsers();
      var usersDto = _mapper.Map<List<UserDto>>(users);

      return Ok(usersDto);
    }

    [HttpGet("{userId}", Name = "GetUser")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetUser(string userId)
    {
      var user = _userRepository.GetUser(userId);
      if (user == null)
      {
        ModelState.AddModelError("CustomError", $"User {userId} doesn't exist");
        return NotFound(ModelState);
      }

      var userDto = _mapper.Map<UserDto>(user);
      return Ok(userDto);
    }

    [AllowAnonymous]
    [HttpPost(Name = "RegisterUser")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RegisterUser([FromBody] CreateUserDto createUserDto)
    {
      if (createUserDto == null || !ModelState.IsValid) return BadRequest("User info is required");
      if (!_userRepository.IsUniqueUser(createUserDto.UserName))
      {
        ModelState.AddModelError("CustomError", "Username alredy exist!");
        return BadRequest(ModelState);
      }

      var user = await _userRepository.Register(createUserDto);
      if (user == null)
      {
        ModelState.AddModelError("CustomError", $"Couldn't create user {createUserDto.UserName}");
        return StatusCode(500, ModelState);
      }

      return CreatedAtRoute("GetUser", new { userId = user.Id }, user);
    }

    [AllowAnonymous]
    [HttpPost("Login", Name = "LoginUser")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> LoginUser(UserLoginDto userLoginDto)
    {
      var loginResponse = await _userRepository.Login(userLoginDto);
      if (loginResponse.User == null)
      {
        ModelState.AddModelError("CustomError", "Invalid credentials");
        return Unauthorized(ModelState);
      }

      return Ok(loginResponse);
    }
  }
}
