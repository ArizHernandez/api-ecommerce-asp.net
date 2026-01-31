using System;
using apiEcommerce.Models;

namespace apiEcommerce.Reporsitory.IRepository;

public interface ICategoryRepository
{
  ICollection<Category> GetCategories();
  ICollection<Category> GetCategoriesPaginated(int pageNumber, int pageSize);
  int GetCategoriesTotal();
  Category? GetCategory(int id);
  bool CategoryExists(int id);
  bool CategoryExists(string name);
  bool CreateCategory(Category category);
  bool UpdateCategory(Category category);
  bool DeleteCategory(Category category);
  bool Save();
}
