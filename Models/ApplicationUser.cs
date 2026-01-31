using System;
using Microsoft.AspNetCore.Identity;

namespace apiEcommerce.Models;

public class ApplicationUser: IdentityUser
{
  public string? Name { get; set; }
}
