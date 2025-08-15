using Microsoft.AspNetCore.Mvc;
using StoreCraft_API.Models.DTOs.ProductDTOs;
using StoreCraft_API.Services;

namespace StoreCraft_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProductDTO>>> GetAllProducts()
        {
            //throw new Exception("Test exception from GetAllProducts");
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTO>> GetProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound($"Product with ID {id} not found");
            }
            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<ProductDTO>> CreateProduct(CreateProductDTO createProductDTO)
        {
            var product = await _productService.CreateProductAsync(createProductDTO);
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ProductDTO>> UpdateProduct(int id, UpdateProductDTO updateProductDTO)
        {
            try
            {
                var product = await _productService.UpdateProductAsync(id, updateProductDTO);
                return Ok(product);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _productService.DeleteProductAsync(id);
            if (!result)
            {
                return NotFound($"Product with ID {id} not found");
            }
            return NoContent();
        }

        [HttpGet("active")]
        public async Task<ActionResult<List<ProductDTO>>> GetActiveProducts()
        {
            var products = await _productService.GetActiveProductsAsync();
            return Ok(products);
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<ProductDTO>>> SearchProducts([FromQuery] string term)
        {
            if (string.IsNullOrEmpty(term))
            {
                return BadRequest("Search term is required");
            }

            var products = await _productService.SearchProductsAsync(term);
            return Ok(products);
        }
    }
}
