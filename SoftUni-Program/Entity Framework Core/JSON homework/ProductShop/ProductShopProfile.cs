using AutoMapper;
using ProductShop.DtoModels.InputDto;
using ProductShop.DtoModels.OutputDto;
using ProductShop.Models;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            CreateMap<UsersInputDto, User>();
            CreateMap<ProductInputDto, Product>();
            CreateMap<CategoryInputDto, Category>();
            CreateMap<CategoryProductInputDto, CategoryProduct>();
            CreateMap<Product, ProductsOutputDto>()
                .ForMember(dest => dest.Seller, fm => fm.MapFrom(src => $"{src.Seller.FirstName} {src.Seller.LastName}"));
           
        }
    }
}
