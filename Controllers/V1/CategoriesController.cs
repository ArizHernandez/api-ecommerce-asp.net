using apiEcommerce.Constants;
using apiEcommerce.Models;
using apiEcommerce.Models.Dtos;
using apiEcommerce.Reporsitory.IRepository;
using Asp.Versioning;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace apiEcommerce.Controllers.V1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    // [EnableCors(PolicyNames.AllowSpecificOrigin)]
    [Authorize(Roles = "Admin")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoriesController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [AllowAnonymous]
        [HttpGet]
        // [ResponseCache(Duration = 10)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        // [EnableCors(PolicyNames.AllowSpecificOrigin)]
        [Obsolete("This method is obsolete. Use categoriesById from v2 instead")]
        public IActionResult getCategories()
        {
            System.Console.WriteLine("Categories load");
            var categories = _categoryRepository.GetCategories();
            var categoriesDto = categories.Adapt<List<CategoryDto>>();
            return Ok(categoriesDto);
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

            var categoryDto = category.Adapt<CategoryDto>();
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

            var category = createCategoryDto.Adapt<Category>();
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

            var category = updateCategoryDto.Adapt<Category>();
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
