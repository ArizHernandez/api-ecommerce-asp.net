using apiEcommerce.Models;
using apiEcommerce.Models.Dtos;
using AutoMapper;

namespace apiEcommerce.Mapping;

public class ProductProfile : Profile
{
  public ProductProfile()
  {
    CreateMap<Product, ProductDto>()
    .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(source => source.Category.Name))
    .ReverseMap();  
    CreateMap<Product, CreateProductDto>().ReverseMap();
    CreateMap<Product, UpdateProductDto>().ReverseMap();
  }
}
