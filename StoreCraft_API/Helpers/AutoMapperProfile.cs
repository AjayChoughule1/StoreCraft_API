using AutoMapper;
using StoreCraft_API.Models;
using StoreCraft_API.Models.DTOs.ProductDTOs;

namespace StoreCraft_API.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<CreateProductDTO, Product>();
            CreateMap<UpdateProductDTO, Product>();

            CreateMap<Product, ProductDTO>()
                .ForMember(dest => dest.CategoryName,
                           opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty));
        }
    }

}
