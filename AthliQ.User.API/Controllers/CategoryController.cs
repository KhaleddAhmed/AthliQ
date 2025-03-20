using AthliQ.Core.DTOs.Category;
using AthliQ.Core.Service.Contract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AthliQ.User.API.Controllers
{
    public class CategoryController : BaseApiController
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("CreateCategory")]
        public async Task<ActionResult> Create(CreateCategoryDto categoryDto)
        {
            var result = await _categoryService.CreateCategoryAsync(categoryDto);
            return Ok(result);
        }

        [HttpGet("GetCategory")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _categoryService.GetCategoryAsync(id);
            return Ok(category);
        }

        [HttpGet("GetCategoryWithSports")]
        public async Task<IActionResult> GetByIdWithSports(int id)
        {
            var category = await _categoryService.GetCategoryWithSportsAsync(id);
            return Ok(category);
        }

        [HttpGet("GetAllCategories")]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateCategory")]
        public async Task<IActionResult> Update(UpdateCategoryDto updateCategoryDto)
        {
            var category = await _categoryService.UpdateCategoryAsync(updateCategoryDto);
            return Ok(category);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteCategory")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryService.DeleteCategoryAsync(id);
            return Ok(category);
        }
    }
}
