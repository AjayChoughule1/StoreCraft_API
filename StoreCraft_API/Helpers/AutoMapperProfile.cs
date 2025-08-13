using AutoMapper;
using StoreCraft_API.Models.DTOs;
using StoreCraft_API.Models;

namespace StoreCraft_API.Helpers
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile() 
        {
            CreateMap<CreateProductDTO, Product>();
            CreateMap<Product, ProductDTO>();
        }
    }
}
