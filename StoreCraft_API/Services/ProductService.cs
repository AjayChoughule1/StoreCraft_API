using StoreCraft_API.Models;
using StoreCraft_API.Models.DTOs;
using StoreCraft_API.Repository;

namespace StoreCraft_API.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<List<ProductDTO>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllProductsAsync();
            return products.Select(p => new ProductDTO
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Stock = p.Stock,
                ImageUrl = p.ImageUrl,
                CategoryName = p.Category?.Name ?? "",
                IsActive = p.IsActive
            }).ToList();
        }

        public async Task<ProductDTO?> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null) return null;

            return new ProductDTO
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                ImageUrl = product.ImageUrl,
                CategoryName = product.Category?.Name ?? "",
                IsActive = product.IsActive
            };
        }

        public async Task<ProductDTO> CreateProductAsync(CreateProductDTO createProductDTO)
        {
            var product = new Product
            {
                Name = createProductDTO.Name,
                Description = createProductDTO.Description,
                Price = createProductDTO.Price,
                Stock = createProductDTO.Stock,
                ImageUrl = createProductDTO.ImageUrl,
                CategoryId = createProductDTO.CategoryId,
                IsActive = true,
                CreatedDate = DateTime.Now
            };

            var createdProduct = await _productRepository.AddProductAsync(product);

            // Get the product with category for DTO
            var productWithCategory = await _productRepository.GetProductByIdAsync(createdProduct.Id);

            return new ProductDTO
            {
                Id = productWithCategory!.Id,
                Name = productWithCategory.Name,
                Description = productWithCategory.Description,
                Price = productWithCategory.Price,
                Stock = productWithCategory.Stock,
                ImageUrl = productWithCategory.ImageUrl,
                CategoryName = productWithCategory.Category?.Name ?? "",
                IsActive = productWithCategory.IsActive
            };
        }

        public async Task<ProductDTO> UpdateProductAsync(int id, UpdateProductDTO updateProductDTO)
        {
            var existingProduct = await _productRepository.GetProductByIdAsync(id);
            if (existingProduct == null)
                throw new ArgumentException($"Product with ID {id} not found");

            existingProduct.Name = updateProductDTO.Name;
            existingProduct.Description = updateProductDTO.Description;
            existingProduct.Price = updateProductDTO.Price;
            existingProduct.Stock = updateProductDTO.Stock;
            existingProduct.ImageUrl = updateProductDTO.ImageUrl;
            existingProduct.CategoryId = updateProductDTO.CategoryId;
            existingProduct.IsActive = updateProductDTO.IsActive;

            var updatedProduct = await _productRepository.UpdateProductAsync(existingProduct);
            var productWithCategory = await _productRepository.GetProductByIdAsync(updatedProduct.Id);

            return new ProductDTO
            {
                Id = productWithCategory!.Id,
                Name = productWithCategory.Name,
                Description = productWithCategory.Description,
                Price = productWithCategory.Price,
                Stock = productWithCategory.Stock,
                ImageUrl = productWithCategory.ImageUrl,
                CategoryName = productWithCategory.Category?.Name ?? "",
                IsActive = productWithCategory.IsActive
            };
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            return await _productRepository.DeleteProductAsync(id);
        }

        public async Task<List<ProductDTO>> GetActiveProductsAsync()
        {
            var products = await _productRepository.GetActiveProductsAsync();
            return products.Select(p => new ProductDTO
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Stock = p.Stock,
                ImageUrl = p.ImageUrl,
                CategoryName = p.Category?.Name ?? "",
                IsActive = p.IsActive
            }).ToList();
        }

        public async Task<List<ProductDTO>> SearchProductsAsync(string searchTerm)
        {
            var products = await _productRepository.SearchProductsAsync(searchTerm);
            return products.Select(p => new ProductDTO
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Stock = p.Stock,
                ImageUrl = p.ImageUrl,
                CategoryName = p.Category?.Name ?? "",
                IsActive = p.IsActive
            }).ToList();
        }
    }
}
