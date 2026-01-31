using apiEcommerce.Data;
using apiEcommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace apiEcommerce.Reporsitory.IRepository;

public class ProductRepository : IProductRepository
{
  private ApplicationDBContext _db;

  public ProductRepository(ApplicationDBContext dbContext)
  {
    _db = dbContext;
  }

  public bool BuyProduct(int productId, int quantity)
  {
    if (productId == 0 || quantity == 0) return false;

    var product = _db.Products.FirstOrDefault((p) => p.ProductId == productId);
    if (product == null || product.Stock < quantity) return false;

    product.Stock -= quantity;
    _db.Products.Update(product);
    return Save();
  }

  public bool CreateProduct(Product product)
  {
    if (product == null) return false;

    product.CreationDate = DateTime.Now;
    product.UpdateDate = DateTime.Now;
    _db.Products.Add(product);
    return Save();
  }

  public bool DeleteProduct(Product product)
  {
    if (product == null) return false;

    _db.Products.Remove(product);
    return Save();
  }

  public Product? GetProduct(int productId)
  {
    if (productId <= 0) return null;
    return _db.Products.Include(p => p.Category).FirstOrDefault(p => p.ProductId == productId);
  }

  public ICollection<Product> GetProducts()
  {
    return _db.Products.Include(p => p.Category).OrderBy(p => p.Name).ToList();
  }

  public ICollection<Product> GetProductsByCategory(int categoryId)
  {
    if (categoryId <= 0) return GetProducts();
    return _db.Products.Where((p) => p.CategoryId == categoryId).OrderBy(p => p.Name).ToList();
  }

  public ICollection<Product> GetProductsInPages(int pageNumber, int pageSize)
  {
    return _db.Products.OrderBy(p => p.ProductId)
      .Skip((pageNumber - 1) * pageSize)
      .Take(pageSize)
      .ToList();
  }

  public int GetTotalProducts()
  {
    return _db.Products.Count();
  }

  public bool ProductExists(int id)
  {
    if (id <= 0) return false;
    return _db.Products.Any(p => p.ProductId == id);
  }

  public bool ProductExists(string name)
  {
    if (string.IsNullOrWhiteSpace(name)) return false;
    return _db.Products.Any(p => p.Name == name);
  }

  public bool Save()
  {
    return _db.SaveChanges() > 0;
  }

  public ICollection<Product> SearchProducts(string searchTerm)
  {
    if (string.IsNullOrWhiteSpace(searchTerm)) return GetProducts();
    var searchTermLower = searchTerm.ToLower().Trim();

    IQueryable<Product> query = _db.Products;
    query = query.Include(p => p.Category).Where(
      p => p.Name.ToLower().Trim().Contains(searchTermLower) ||
            p.Description.ToLower().Trim().Contains(searchTermLower)
    ).OrderBy(p => p.Name);
    return query.ToList();
  }

  public bool UpdateProduct(Product product)
  {
    if (product == null) return false;

    product.UpdateDate = DateTime.Now;
    _db.Products.Update(product);
    return Save();
  }
}
