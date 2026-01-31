using apiEcommerce.Models;
using apiEcommerce.Models.Dtos;
using AutoMapper;

namespace apiEcommerce.Mapping;

public class CategoryProfile: Profile
{
  public CategoryProfile()
  {
    CreateMap<Category, CategoryDto>().ReverseMap();
    CreateMap<Category, CreateCategoryDto>().ReverseMap();
  }
}
