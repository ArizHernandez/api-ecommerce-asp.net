using apiEcommerce.Models;

namespace apiEcommerce.Reporsitory.IRepository;

public interface IProductRepository
{
  ICollection<Product> GetProducts();
  ICollection<Product> GetProductsByCategory(int categoryId);
  ICollection<Product> SearchProducts(string searchTerm);
  Product? GetProduct(int productId);
  bool BuyProduct(int productId, int quantity);
  bool ProductExists(int id);
  bool ProductExists(string name);
  bool CreateProduct(Product product);
  bool UpdateProduct(Product product);
  bool DeleteProduct(Product product);
  bool Save();
}
