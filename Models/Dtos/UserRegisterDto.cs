using System;
using System.ComponentModel.DataAnnotations;

namespace apiEcommerce.Models.Dtos;

public class UserRegisterDto
{
  public string? Id { get; set; }
  public string? Name { get; set; }
  public required string UserName { get; set; }
  public string? Role { get; set; }
}
