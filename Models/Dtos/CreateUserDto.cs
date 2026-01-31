using System;
using System.ComponentModel.DataAnnotations;

namespace apiEcommerce.Models.Dtos;

public class CreateUserDto
{
  [Required(ErrorMessage = "Field Name is required")]
  public string Name { get; set; } = string.Empty;
  [Required(ErrorMessage = "Field UserName is required")]
  public string UserName { get; set; } = string.Empty;
  [Required(ErrorMessage = "Field Password is required")]
  public string Password { get; set; } = string.Empty;
  [Required(ErrorMessage = "Field Role is required")]
  public string Role { get; set; } = string.Empty;
}
