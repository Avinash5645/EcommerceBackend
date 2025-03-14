using Application.DTOs;
using Application.Interfaces;
using Core.Comman;
using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class CategoryService : ICategoryService
{
    private readonly IGenericRepository<Category> _categoryRepository;

    public CategoryService(IGenericRepository<Category> categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<PagedResultDto<CategoryDto>> GetAllCategoriesAsync(PaginationParams paginationParams)
    {
        var query = _categoryRepository.AsQueryable();

        var totalCount = await query.CountAsync();
        var categories = await query
            .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
              //  Description = c.Description
            })
            .ToListAsync();

        return new PagedResultDto<CategoryDto>
        {
            Items = categories,
            TotalCount = totalCount
        };
    }

    public async Task<CategoryDto> GetCategoryByIdAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id)
            ?? throw new Exception("Category not found");

        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        };
    }

    public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto)
    {
        var category = new Category
        {
            Name = dto.Name
        };

        await _categoryRepository.AddAsync(category);

        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name
        };
    }

    public async Task<CategoryDto> UpdateCategoryAsync(int id, UpdateCategoryDto dto)
    {
        var category = await _categoryRepository.GetByIdAsync(id)
            ?? throw new Exception("Category not found");

        category.Name = dto.Name;
        category.Description = dto.Description;
        _categoryRepository.Update(category);

        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description=category.Description
        };
    }

    public async Task DeleteCategoryAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id)
            ?? throw new Exception("Category not found");

        _categoryRepository.Delete(category);
    }
}
