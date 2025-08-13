using StoreCraft_API.Models;

namespace StoreCraft_API.Repository
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllProductsAsync();
        Task<Product?> GetProductByIdAsync(int id);
        Task<Product> AddProductAsync(Product product);
        Task<Product> UpdateProductAsync(Product product);
        Task<bool> DeleteProductAsync(int id);
        Task<List<Product>> GetActiveProductsAsync();
        Task<List<Product>> SearchProductsAsync(string searchTerm);
    }
}
