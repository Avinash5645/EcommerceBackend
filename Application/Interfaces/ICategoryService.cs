using Application.DTOs;
using Core.Comman;

namespace Application.Interfaces;

public interface ICategoryService
{
    Task<PagedResultDto<CategoryDto>> GetAllCategoriesAsync(PaginationParams paginationParams);
    Task<CategoryDto> GetCategoryByIdAsync(int id);
    Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto);
    Task<CategoryDto> UpdateCategoryAsync(int id, UpdateCategoryDto dto);
    Task DeleteCategoryAsync(int id);
}
