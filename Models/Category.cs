using System;
using System.ComponentModel.DataAnnotations;

namespace apiEcommerce.Models;

public class Category
{
  [Key]
  public int Id { get; set; }
  
  [Required(ErrorMessage = "Nombre es requerido")]
  public string Name { get; set; } = string.Empty;

  [Required(ErrorMessage = "CreationDate es requerido")]
  public DateTime CreationDate { get; set; }
}
