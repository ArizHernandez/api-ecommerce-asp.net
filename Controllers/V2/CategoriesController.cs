using apiEcommerce.Constants;
using apiEcommerce.Models;
using apiEcommerce.Models.Dtos;
using apiEcommerce.Reporsitory.IRepository;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace apiEcommerce.Controllers.V2
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    [ApiController]
    // [EnableCors(PolicyNames.AllowSpecificOrigin)]
    [Authorize(Roles = "Admin")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoriesController(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet]
        // [ResponseCache(Duration = 10)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        // [EnableCors(PolicyNames.AllowSpecificOrigin)]
        public IActionResult getCategoriesOrderById()
        {
            System.Console.WriteLine("Categories load");
            var categories = _categoryRepository.GetCategories().OrderBy(cat => cat.Id);
            var categoriesDto = new List<CategoryDto>();

            foreach (var category in categories)
            {
                categoriesDto.Add(_mapper.Map<CategoryDto>(category));
            }

            return Ok(categoriesDto);
        }

        [AllowAnonymous]
        [HttpGet("Paginated", Name = "GetCategoriesPaginated")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetCategoriesPaginated([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            if (pageNumber < 0 || pageSize < 0)
            {
                ModelState.AddModelError("CustomError", "Pagination info are invalid");
                return BadRequest(ModelState);
            }

            var totalCategories = _categoryRepository.GetCategoriesTotal();
            var totalPages = (int)Math.Ceiling((float)totalCategories / pageSize);
            if (totalPages < 0)
            {
                return NotFound("Theres not more data to find");
            }

            var categories = _categoryRepository.GetCategoriesPaginated(pageNumber, pageSize);
            var categoriesDto = _mapper.Map<List<Category>>(categories);
            return Ok(new PaginationResponse<Category>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                Data = categories,
            });
        }

        [AllowAnonymous]
        [HttpGet("{id:int}", Name = "GetCategory")]
        // [ResponseCache(Duration = 10)]
        [ResponseCache(CacheProfileName = CacheProfiles.Defaul10)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult getCategory(int id)
        {
            System.Console.WriteLine($"Categoriy with id: {id} at {DateTime.Now}");
            var category = _categoryRepository.GetCategory(id);
            System.Console.WriteLine($"Response with id: {id}");
            if (category == null)
            {
                return NotFound($"Category with id \"{id}\" not exist");
            }

            var categoryDto = _mapper.Map<CategoryDto>(category);
            return Ok(categoryDto);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult createCategory([FromBody] CreateCategoryDto createCategoryDto)
        {
            if (createCategoryDto == null) return BadRequest(ModelState);
            if (_categoryRepository.CategoryExists(createCategoryDto.Name))
            {
                ModelState.AddModelError("CustomError", "Category already exist");

                return Conflict(ModelState);
            }

            var category = _mapper.Map<Category>(createCategoryDto);
            if (!_categoryRepository.CreateCategory(category))
            {
                ModelState.AddModelError("CustomError", $"Something goes wrong creating {category.Name}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetCategory", new { id = category.Id }, category);
        }

        [HttpPatch("{id:int}", Name = "UpdateCategory")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult UpdateCategory([FromBody] CreateCategoryDto updateCategoryDto, int id)
        {
            if (!_categoryRepository.CategoryExists(id)) return NotFound();
            if (updateCategoryDto == null) return BadRequest(ModelState);

            var category = _mapper.Map<Category>(updateCategoryDto);
            if (_categoryRepository.CategoryExists(category.Name)) return Conflict($"Category {category.Name} already exist");

            category.Id = id;
            if (!_categoryRepository.UpdateCategory(category))
            {
                ModelState.AddModelError("CustomError", $"Something goes wrong updating {category.Name}");
                return StatusCode(500, ModelState);
            }

            return Ok("Category updated!");
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult DeleteCategory(int id)
        {
            var category = _categoryRepository.GetCategory(id);
            if (category == null) return NotFound("Category not exist");

            if (!_categoryRepository.DeleteCategory(category))
            {
                ModelState.AddModelError("CustomError", $"Something goes wrong deleting {category.Name}");
                return StatusCode(500, ModelState);
            }


            return Ok("Category deleted");
        }
    }
}
