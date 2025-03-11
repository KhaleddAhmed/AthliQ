using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AthliQ.Core.DTOs.Category;
using AthliQ.Core.Responses;

namespace AthliQ.Core.Service.Contract
{
    public interface ICategoryService
    {
        Task<GenericResponse<GetCategoryDto>> CreateCategoryAsync(CreateCategoryDto createCategoryDto);

        Task<GenericResponse<GetCategoryDto>> GetCategoryAsync(int id);
        Task<GenericResponse<GetCategoryWithSportsDto>> GetCategoryWithSportsAsync(int id);

        Task<GenericResponse<List<GetAllCategoriesDto>>> GetAllCategoriesAsync();

        Task<GenericResponse<GetCategoryDto>> UpdateCategoryAsync(UpdateCategoryDto updateCategoryDto);

        Task<GenericResponse<bool>> DeleteCategoryAsync(int id);
    }
}
