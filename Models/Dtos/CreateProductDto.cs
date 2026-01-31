using System;

namespace apiEcommerce.Models.Dtos;

public class CreateProductDto
{
  public string Description { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;
  public decimal Price { get; set; }
  public string? ImageUrl { get; set; }
  public IFormFile? Image { get; set; }
  public string SKU { get; set; } = string.Empty; //PRODUCT-001-BLK-M
  public int Stock { get; set; }
  public int CategoryId { get; set; }
}
