using System;
using System.ComponentModel.DataAnnotations;

namespace apiEcommerce.Models.Dtos;

public class UserLoginDto
{
  [Required(ErrorMessage = "Field UserName is required")]
  public string UserName { get; set; } = string.Empty;
  [Required(ErrorMessage = "Field Password is required")]
  public string Password { get; set; } = string.Empty;
}
