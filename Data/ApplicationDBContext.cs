using apiEcommerce.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace apiEcommerce.Data;

// public class ApplicationDBContext : DbContext
public class ApplicationDBContext : IdentityDbContext<ApplicationUser>
{
  public DbSet<Category> Categories { get; set; }
  public DbSet<Product> Products { get; set; }
  public DbSet<User> Users { get; set; }
  public DbSet<ApplicationUser> ApplicationUsers { get; set; }

  public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
  { }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
  }
}
