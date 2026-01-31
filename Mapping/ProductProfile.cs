
using apiEcommerce.Models;
using apiEcommerce.Models.Dtos;
using Mapster;

namespace apiEcommerce.Mapping;

public static class ProductProfile
{
    public static void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Product, ProductDto>()
            .Map(dest => dest.CategoryName, src => src.Category != null ? src.Category.Name : null)
            .TwoWays();
        config.NewConfig<Product, CreateProductDto>().TwoWays();
        config.NewConfig<Product, UpdateProductDto>().TwoWays();
    }
}
