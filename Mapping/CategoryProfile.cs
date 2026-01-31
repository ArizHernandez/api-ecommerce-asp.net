
using apiEcommerce.Models;
using apiEcommerce.Models.Dtos;
using Mapster;

namespace apiEcommerce.Mapping;

public static class CategoryProfile
{
    public static void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Category, CategoryDto>().TwoWays();
        config.NewConfig<Category, CreateCategoryDto>().TwoWays();
    }
}
