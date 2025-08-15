using AutoMapper;
using StoreCraft_API.Models;
using StoreCraft_API.Models.DTOs.ProductDTOs;
using StoreCraft_API.Repository;

namespace StoreCraft_API.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<List<ProductDTO>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllProductsAsync();
            if (products == null || !products.Any())
            {
                return new List<ProductDTO>();
            }
            var productDto = _mapper.Map<List<ProductDTO>>(products);
            return productDto;          
        }

        public async Task<ProductDTO?> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null)
            {
                return null;
            }
            var productDto = _mapper.Map<ProductDTO>(product);
            return productDto;
        }

        public async Task<ProductDTO> CreateProductAsync(CreateProductDTO createProductDTO)
        {          
            var product = _mapper.Map<Product>(createProductDTO);

            var createdProduct = await _productRepository.AddProductAsync(product);

            var productWithCategory = await _productRepository.GetProductByIdAsync(createdProduct.Id);

            var productDto = _mapper.Map<ProductDTO>(productWithCategory);
            return productDto;
        }


        public async Task<ProductDTO> UpdateProductAsync(int id, UpdateProductDTO updateProductDTO)
        {
            var existingProduct = await _productRepository.GetProductByIdAsync(id);
            if (existingProduct == null)
                throw new ArgumentException($"Product with ID {id} not found");
            _mapper.Map(updateProductDTO, existingProduct);

            var updatedProduct = await _productRepository.UpdateProductAsync(existingProduct);
            var productWithCategory = await _productRepository.GetProductByIdAsync(updatedProduct.Id);

            return _mapper.Map<ProductDTO>(productWithCategory);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            return await _productRepository.DeleteProductAsync(id);
        }

        public async Task<List<ProductDTO>> GetActiveProductsAsync()
        {
            var products = await _productRepository.GetActiveProductsAsync();
            return _mapper.Map<List<ProductDTO>>(products);           
        }

        public async Task<List<ProductDTO>> SearchProductsAsync(string searchTerm)
        {
            var products = await _productRepository.SearchProductsAsync(searchTerm);
            return _mapper.Map<List<ProductDTO>>(products);           
        }
    }
}
