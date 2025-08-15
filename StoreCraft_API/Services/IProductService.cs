using StoreCraft_API.Models.DTOs.ProductDTOs;

namespace StoreCraft_API.Services
{
    public interface IProductService
    {
        Task<List<ProductDTO>> GetAllProductsAsync();
        Task<ProductDTO?> GetProductByIdAsync(int id);
        Task<ProductDTO> CreateProductAsync(CreateProductDTO createProductDTO);
        Task<ProductDTO> UpdateProductAsync(int id, UpdateProductDTO updateProductDTO);
        Task<bool> DeleteProductAsync(int id);
        Task<List<ProductDTO>> GetActiveProductsAsync();
        Task<List<ProductDTO>> SearchProductsAsync(string searchTerm);
    }
}
