using AutoMapper;
using Identity.Core.Application.DTOs.Product;
using Identity.Core.Application.DTOs.ProductCategory;
using Identity.Core.Domain;

namespace Identity.Core.Application.Profiles
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ProductCategory, ProductCategoryDto>()
                .ForMember(x => x.CreationDate, options =>
                {
                    options.MapFrom(src => src.CreationDate.ToShortDateString());
                }).ReverseMap();
            CreateMap<ProductCategory, CreateProductCategoryDto>().ReverseMap();
            CreateMap<ProductCategory, UpdateProductCategoryDto>().ReverseMap();

            CreateMap<Product, ProductDto>().ForMember(x => x.CreationDate, options =>
            {
                options.MapFrom(src => src.CreationDate.ToShortDateString());
            }).ReverseMap();
            CreateMap<Product, CreateProductDto>().ReverseMap();
            CreateMap<Product, UpdateProductDto>().ReverseMap();
        }
    }
}
