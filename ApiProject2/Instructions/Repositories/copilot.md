# Repositories Layer - Quick Reference

## Purpose
Data access abstraction. Encapsulates database queries, manages Entity Framework DbContext, and provides CRUD operations to services.

## Guidelines
- Use `async/await` for all database operations (`.ToListAsync()`, `.FirstOrDefaultAsync()`, etc.)
- Use `.Include()` for eager loading related data
- Use `.AsNoTracking()` for read-only queries to improve performance
- Implement pagination in GetAll methods
- Return `null` for not-found scenarios, let service layer handle exceptions
- Use `.Where()` and `.Select()` to filter/project at database level, not in memory
- Always validate input parameters
- Let database exceptions propagate to service layer
- Use `FindAsync()` when checking by primary key (faster)

## Security Notes
- Use parameterized queries (Entity Framework does this automatically)
- Always use LINQ methods, never string concatenation for queries
- Filter by user/tenant context to prevent unauthorized data access
- Never expose hashed passwords or sensitive fields in results
- Use `.Select()` to exclude sensitive columns from queries

## Testing Recommendations
- Use in-memory database for testing (`.UseInMemoryDatabase()`)
- Create test fixtures with seed data
- Test pagination, filtering, and sorting
- Test null returns for missing records
- Verify related data is loaded with `.Include()`

## Code Examples

### Repository Interface & Implementation
```csharp
public interface ICategoryRepository
{
    Task<Category> GetByIdAsync(int id);
    Task<IEnumerable<Category>> GetAllAsync(int pageNumber = 1, int pageSize = 10);
    Task<Category> GetByNameAsync(string name);
    Task<Category> AddAsync(Category entity);
    Task<bool> UpdateAsync(Category entity);
    Task<bool> DeleteAsync(int id);
}

public class CategoryRepository : ICategoryRepository
{
    private readonly ProjectContext _context;

    public CategoryRepository(ProjectContext context)
    {
        _context = context;
    }

    public async Task<Category> GetByIdAsync(int id)
    {
        return await _context.Categories.FindAsync(id);
    }

    public async Task<IEnumerable<Category>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
    {
        return await _context.Categories
            .OrderBy(x => x.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Category> GetByNameAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;
        return await _context.Categories.FirstOrDefaultAsync(x => x.Name == name.Trim());
    }

    public async Task<Category> AddAsync(Category entity)
    {
        _context.Categories.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> UpdateAsync(Category entity)
    {
        _context.Categories.Update(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.Categories.FindAsync(id);
        if (entity == null)
            return false;
        _context.Categories.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
```

### Pagination & Filtering
```csharp
public async Task<IEnumerable<Category>> GetBySearchAsync(string search, int pageNumber = 1, int pageSize = 10)
{
    var query = _context.Categories.AsQueryable();

    if (!string.IsNullOrWhiteSpace(search))
        query = query.Where(x => x.Name.Contains(search) || x.Description.Contains(search));

    return await query
        .OrderBy(x => x.Name)
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();
}
```

### Eager Loading Related Data
```csharp
public async Task<Category> GetByIdWithItemsAsync(int id)
{
    return await _context.Categories
        .Include(x => x.Items)
        .FirstOrDefaultAsync(x => x.Id == id);
}
```
