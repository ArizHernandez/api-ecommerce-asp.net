using System;
using System.ComponentModel.DataAnnotations;

namespace apiEcommerce.Models.Dtos;

public class UserChangePasswordDto
{
  [Required]
  public int Id { get; set; }
  [Required]
  public string Password { get; set; } = string.Empty;
}
