# Services Layer - Quick Reference

## Purpose
Core business logic implementation. Validates business rules, orchestrates operations, transforms data (DTO ↔ Model), and coordinates with repositories.

## Guidelines
- Use async/await for all I/O operations
- Inject repositories and other services via constructor
- Use interfaces (e.g., `IServiceName`) for loose coupling
- Validate business rules (beyond data type validation)
- Map DTOs to domain models before repository calls
- Handle repository exceptions and throw meaningful business exceptions
- Log operations with appropriate context
- Return or throw meaningful exceptions
- Use `Task<T>` return types for async operations

## Security Notes
- Implement authorization checks (verify user permissions)
- Don't log passwords, tokens, or sensitive data
- Validate all inputs, even if controller already validated
- Sanitize string inputs before using in queries
- Use transactions for multi-step operations that must be atomic

## Testing Recommendations
- Mock repositories using Moq
- Test business logic validation (success and failure cases)
- Verify exceptions are thrown for invalid operations
- Verify repository methods are called with correct parameters
- Test with both valid and invalid data scenarios

## Code Examples

### Service Interface & Implementation
```csharp
public interface ICategoryService
{
    Task<CategoryDto> GetByIdAsync(int id);
    Task<IEnumerable<CategoryDto>> GetAllAsync(int pageNumber, int pageSize);
    Task<CategoryDto> CreateAsync(CreateCategoryDto dto);
    Task<bool> UpdateAsync(int id, UpdateCategoryDto dto);
    Task<bool> DeleteAsync(int id);
}

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repository;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(ICategoryRepository repository, ILogger<CategoryService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<CategoryDto> GetByIdAsync(int id)
    {
        _logger.LogInformation($"Retrieving category {id}");
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
            throw new KeyNotFoundException($"Category {id} not found");
        return MapToDto(entity);
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
    {
        // Validate business rule
        var existing = await _repository.GetByNameAsync(dto.Name);
        if (existing != null)
            throw new InvalidOperationException($"Category '{dto.Name}' already exists");

        _logger.LogInformation($"Creating category: {dto.Name}");
        var entity = new Category { Name = dto.Name, Description = dto.Description };
        var created = await _repository.AddAsync(entity);
        return MapToDto(created);
    }

    private CategoryDto MapToDto(Category entity) =>
        new() { Id = entity.Id, Name = entity.Name, Description = entity.Description };
}
```

### Business Logic with Validation
```csharp
public async Task<bool> UpdateAsync(int id, UpdateCategoryDto dto)
{
    var entity = await _repository.GetByIdAsync(id);
    if (entity == null)
        return false;

    // Check for duplicate name if changing
    if (dto.Name != entity.Name)
    {
        var duplicate = await _repository.GetByNameAsync(dto.Name);
        if (duplicate != null)
            throw new InvalidOperationException($"Another category with name '{dto.Name}' exists");
    }

    entity.Name = dto.Name ?? entity.Name;
    entity.Description = dto.Description ?? entity.Description;
    await _repository.UpdateAsync(entity);
    return true;
}
```
