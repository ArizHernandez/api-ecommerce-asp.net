using apiEcommerce.Models;
using apiEcommerce.Models.Dtos;
using apiEcommerce.Reporsitory.IRepository;
using Asp.Versioning;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace apiEcommerce.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersionNeutral]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ProductsController : ControllerBase
    {
        private IProductRepository _productRepository;
        private ICategoryRepository _categoryRepository;
        public ProductsController(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetProducts()
        {
            var products = _productRepository.GetProducts();
            var productsDto = products.Adapt<List<ProductDto>>();
            return Ok(productsDto);
        }

        [Authorize]
        [HttpGet("Paged", Name = "GetProductInPage")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetProductInPage([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                ModelState.AddModelError("CustomError", "Pagination info are invalid");
                return BadRequest(ModelState);
            }
            var totalProducts = _productRepository.GetTotalProducts();
            var totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);
            if (pageNumber > totalPages)
            {
                return NotFound("There's not more pages available");
            }

            var products = _productRepository.GetProductsInPages(pageNumber, pageSize);
            var productsDto = products.Adapt<List<ProductDto>>();
            var paginationResponse = new PaginationResponse<ProductDto>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                Data = productsDto,
            };
            return Ok(paginationResponse);
        }

        [Authorize]
        [HttpGet("{productId:int}", Name = "GetProduct")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetProduct(int productId)
        {
            var product = _productRepository.GetProduct(productId);
            if (product == null)
            {
                return NotFound($"Product {productId} doesn't exist");
            }

            var productDto = product.Adapt<ProductDto>();
            return Ok(productDto);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateProduct([FromForm] CreateProductDto createProductDto)
        {
            if (createProductDto == null) return BadRequest(ModelState);
            if (_productRepository.ProductExists(createProductDto.Name))
            {
                ModelState.AddModelError("CustomError", $"Product {createProductDto.Name} alredy exists");
                return BadRequest(ModelState);
            }
            if (!_categoryRepository.CategoryExists(createProductDto.CategoryId))
            {
                ModelState.AddModelError("CustomError", "Category doesn't exist");
                return BadRequest(ModelState);
            }

            var product = createProductDto.Adapt<Product>();
            UploadProductImage(createProductDto, product);
            if (!_productRepository.CreateProduct(product))
            {
                ModelState.AddModelError("CustomError", $"Something goes wrong creating ${product.Name}");
                return StatusCode(500, ModelState);
            }

            var createdProduct = _productRepository.GetProduct(product.ProductId);
            var productDto = createdProduct.Adapt<ProductDto>();
            return CreatedAtRoute("GetProduct", new { productId = product.ProductId }, productDto);
        }

        private void UploadProductImage(dynamic productDto, Product product)
        {
            if (productDto.Image != null)
            {
                string fileName = $"{product.ProductId}{Guid.NewGuid()}{Path.GetExtension(productDto.Image.FileName)}";
                var imageFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "products-images");
                if (!Directory.Exists(imageFolder))
                {
                    Directory.CreateDirectory(imageFolder);
                }
                var filePath = Path.Combine(imageFolder, fileName);
                FileInfo file = new FileInfo(filePath);
                if (file.Exists)
                {
                    file.Delete();
                }
                using var fileStream = new FileStream(filePath, FileMode.Create);
                productDto.Image.CopyTo(fileStream);
                string baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                product.ImageUrl = $"{baseUrl}/products-images/{fileName}";
                product.ImageUrlLocale = filePath;
            }
            else
            {
                product.ImageUrl = "https://placehold.co/600x400";
            }
        }

        [HttpPost("ByCategory/{categoryId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetProductByCategory(int categoryId)
        {
            if (categoryId == 0)
            {
                ModelState.AddModelError("CustomError", "CategoryId must be sent");
                return BadRequest(ModelState);
            }

            var products = _productRepository.GetProductsByCategory(categoryId);
            var productsDto = products.Adapt<List<ProductDto>>();
            return Ok(productsDto);
        }

        [HttpGet("{searchTerm}", Name = "SearchProduct")]
        public IActionResult SearchProduct(string searchTerm)
        {
            var products = _productRepository.SearchProducts(searchTerm);
            var productsDto = products.Adapt<List<ProductDto>>();
            return Ok(productsDto);
        }

        [HttpPatch("{productId:int}", Name = "UpdateProduct")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateProduct([FromForm] UpdateProductDto updateProductDto, int productId)
        {
            if (updateProductDto == null || productId == 0)
            {
                ModelState.AddModelError("CustomError", "Product info is required");
                return BadRequest(ModelState);
            }

            if (!_productRepository.ProductExists(productId))
            {
                ModelState.AddModelError("CustomError", "Product doesn't exist");
                return BadRequest(ModelState);
            }

            var product = updateProductDto.Adapt<Product>();
            product.ProductId = productId;
            UploadProductImage(updateProductDto, product);
            if (!_productRepository.UpdateProduct(product))
            {
                ModelState.AddModelError("CustomError", $"Error updating product {product.Name}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetProduct", new { productId = product.CategoryId }, product);
        }

        [HttpDelete("{productId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteProduct(int productId)
        {
            if (productId == 0)
            {
                ModelState.AddModelError("CustomError", "ProductId is required");
                return BadRequest(ModelState);
            }

            var product = _productRepository.GetProduct(productId);
            if (product == null) return NotFound("Product doesn't exist");

            if (!_productRepository.DeleteProduct(product))
            {
                ModelState.AddModelError("CustomError", "Product coudn't delete");
                return StatusCode(500, ModelState);
            }

            return Ok($"Product {product.Name} deleted!");
        }

        [HttpPost("BuyProduct")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult BuyProduct([FromBody] BuyProductDto buyProductDto)
        {
            if (buyProductDto == null) return BadRequest("Product info is required");
            if (!_productRepository.ProductExists(buyProductDto.ProductId))
            {
                ModelState.AddModelError("CustomError", "Product doesn't exist");
                return StatusCode(500, ModelState);
            }
            if (!_productRepository.BuyProduct(buyProductDto.ProductId, buyProductDto.Quantity))
            {
                ModelState.AddModelError("CustomError", "Error buying product");
                return StatusCode(500, ModelState);
            }

            return Ok("Product purchased!");
        }
    }
}
