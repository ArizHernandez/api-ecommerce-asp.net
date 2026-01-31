using apiEcommerce.Data;
using apiEcommerce.Models;

namespace apiEcommerce.Reporsitory.IRepository;

public class CategoryRepository : ICategoryRepository
{
  private readonly ApplicationDBContext _db;

  public CategoryRepository(ApplicationDBContext dBContext)
  {
    _db = dBContext;
  }

  public bool CategoryExists(int id)
  {
    return _db.Categories.Any(c => c.Id == id);
  }

  public bool CategoryExists(string name)
  {
    return _db.Categories.Any(c => c.Name.ToLower().Trim() == name.ToLower().Trim());
  }

  public bool CreateCategory(Category category)
  {
    category.CreationDate = DateTime.Now;
    _db.Categories.Add(category);
    return Save();
  }

  public bool DeleteCategory(Category category)
  {
    _db.Categories.Remove(category);
    return Save();
  }

  public ICollection<Category> GetCategories()
  {
    return _db.Categories.OrderBy(c => c.Name).ToList();
  }

  public Category? GetCategory(int id)
  {
    return _db.Categories.FirstOrDefault(c => c.Id == id);
  }

  public bool Save()
  {
    return _db.SaveChanges() >= 0;
  }

  public bool UpdateCategory(Category category)
  {
    category.CreationDate = DateTime.Now;
    _db.Categories.Update(category);
    return Save();
  }
}
