# Repositories Layer Instructions

## Purpose
The Repository layer provides data access abstraction. It is responsible for:
- **Data Persistence**: Saving, updating, and deleting records in the database
- **Data Retrieval**: Querying the database with various filters, sorting, and pagination options
- **Query Abstraction**: Encapsulating database queries so the service layer doesn't know about database implementation
- **Database Connection Management**: Managing Entity Framework DbContext lifecycle
- **Transaction Support**: Enabling atomic operations across multiple entities
- **Lazy Loading Prevention**: Controlling entity relationships and eager loading
- **Query Optimization**: Implementing efficient queries to prevent N+1 problems
- **Specification Pattern**: Supporting complex query logic in a maintainable way

Repositories act as an in-memory collection abstraction, allowing services to work with entities as if they were simple collections without worrying about the underlying database.

---

## Where to Look
- **Location**: `ApiProject/Repositories/` folder (create interfaces and implementations here)
- **DbContext**: `ApiProject/Data/ProjectContext.cs` (where Entity Framework models are defined)
- **Database Models**: `ApiProject/Models/` folder (entities mapped to database tables)
- **Services**: `ApiProject/Services/` folder (calls repository methods)

---

## Guidelines

### 1. **Naming Conventions**
- Interface names should be prefixed with `I` (e.g., `ICategoryRepository`, `IProductRepository`)
- Implementation class names should match the interface without `I` (e.g., `CategoryRepository`, `ProductRepository`)
- Method names should use Repository pattern conventions:
  - `GetByIdAsync(id)` - retrieve single item by primary key
  - `GetAllAsync(pageNumber, pageSize)` - retrieve all items with pagination
  - `AddAsync(entity)` - add new entity
  - `UpdateAsync(entity)` - update existing entity
  - `DeleteAsync(id)` - delete entity by id
  - `FindAsync(predicate)` - find items matching condition
- Use `Async` suffix for all asynchronous methods

### 2. **CRUD Operations**
- **Create (Add)**: Use `AddAsync()` method, return the created entity with generated ID
- **Read**: Implement multiple `Get` methods for different query scenarios
- **Update**: Implement `UpdateAsync()` method, ensure tracked entities
- **Delete**: Implement `DeleteAsync()` method, consider soft deletes for sensitive data

### 3. **Entity Framework Best Practices**
- Use `DbSet<T>` for type-safe query operations
- Always use `async` methods (`ToListAsync()`, `FirstOrDefaultAsync()`, etc.)
- Implement `SaveChangesAsync()` explicitly in a Unit of Work pattern or at the service level
- Use `.Include()` for eager loading related data
- Avoid `.ToList()` before filtering - let database filter
- Use `.AsNoTracking()` for read-only queries to improve performance

### 4. **Query Patterns**
- Use IQueryable for composable queries
- Separate query building from execution
- Implement pagination at the repository level
- Filter at the database level, not in-memory
- Use expressions for reusable query logic

### 5. **Dependency Injection**
- Inject `ProjectContext` via constructor (DbContext)
- Make dependencies `readonly`
- Use logging injections when needed for debugging

### 6. **Error Handling**
- Let database exceptions propagate to service layer
- Don't catch and suppress database errors in repository
- Use specific exception types when meaningful
- Log database operations when debugging

### 7. **Performance Optimization**
- Use pagination to limit result sets
- Implement strategic eager loading with `.Include()`
- Use `.Select()` to avoid loading unnecessary data
- Consider `.AsNoTracking()` for read-only operations
- Monitor and optimize slow queries

---

## Signature Conventions

### Basic Repository Interface & Implementation

```csharp
using ApiProject.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiProject.Repositories
{
    public interface IYourResourceRepository
    {
        // Read operations
        Task<YourResource> GetByIdAsync(int id);
        Task<IEnumerable<YourResource>> GetAllAsync(int pageNumber = 1, int pageSize = 10);
        Task<YourResource> GetByNameAsync(string name);
        Task<IEnumerable<YourResource>> FindAsync(Func<YourResource, bool> predicate);
        
        // Write operations
        Task<YourResource> AddAsync(YourResource entity);
        Task<bool> UpdateAsync(YourResource entity);
        Task<bool> DeleteAsync(int id);
        
        // Existence checks
        Task<bool> ExistsAsync(int id);
        Task<int> GetCountAsync();
    }

    public class YourResourceRepository : IYourResourceRepository
    {
        private readonly ProjectContext _context;
        private readonly ILogger<YourResourceRepository> _logger;

        public YourResourceRepository(ProjectContext context, ILogger<YourResourceRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Implementation methods follow
    }
}
```

### Standard Method Patterns

#### Get By ID (Single Item)
```csharp
public async Task<YourResource> GetByIdAsync(int id)
{
    _logger.LogInformation($"Fetching resource with id: {id}");
    
    var entity = await _context.YourResources
        .Include(x => x.RelatedEntity)  // Eager load related data if needed
        .FirstOrDefaultAsync(x => x.Id == id);
    
    return entity;
}
```

#### Get All with Pagination
```csharp
public async Task<IEnumerable<YourResource>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
{
    _logger.LogInformation($"Fetching all resources - Page: {pageNumber}, Size: {pageSize}");
    
    // Validate pagination parameters
    if (pageNumber < 1) pageNumber = 1;
    if (pageSize < 1) pageSize = 10;
    if (pageSize > 100) pageSize = 100;  // Max limit

    var entities = await _context.YourResources
        .OrderBy(x => x.CreatedAt)  // Consistent ordering
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();
    
    return entities;
}
```

#### Get With Complex Filtering
```csharp
public async Task<IEnumerable<YourResource>> GetByCategoryAsync(
    string category, 
    string sortBy = "name", 
    bool ascending = true)
{
    _logger.LogInformation($"Fetching resources by category: {category}");
    
    var query = _context.YourResources
        .AsNoTracking()  // Read-only, don't track changes
        .Where(x => x.Category == category);

    // Dynamic sorting
    query = sortBy.ToLower() switch
    {
        "price" => ascending 
            ? query.OrderBy(x => x.Price)
            : query.OrderByDescending(x => x.Price),
        "name" => ascending
            ? query.OrderBy(x => x.Name)
            : query.OrderByDescending(x => x.Name),
        _ => query.OrderBy(x => x.Id)
    };

    return await query.ToListAsync();
}
```

#### Get By Name (Unique Field)
```csharp
public async Task<YourResource> GetByNameAsync(string name)
{
    _logger.LogInformation($"Fetching resource with name: {name}");
    
    if (string.IsNullOrWhiteSpace(name))
        return null;

    var entity = await _context.YourResources
        .AsNoTracking()
        .FirstOrDefaultAsync(x => x.Name == name.Trim());
    
    return entity;
}
```

#### Find with Predicate (Generic Search)
```csharp
public async Task<IEnumerable<YourResource>> FindAsync(
    Func<YourResource, bool> predicate, 
    int pageNumber = 1, 
    int pageSize = 10)
{
    _logger.LogInformation("Searching resources with custom predicate");
    
    var entities = _context.YourResources
        .AsNoTracking()
        .Where(predicate)
        .OrderBy(x => x.CreatedAt)
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToList();
    
    return entities;
}
```

#### Add/Create New Entity
```csharp
public async Task<YourResource> AddAsync(YourResource entity)
{
    if (entity == null)
        throw new ArgumentNullException(nameof(entity));

    _logger.LogInformation($"Adding new resource: {entity.Name}");
    
    entity.CreatedAt = DateTime.UtcNow;
    
    try
    {
        _context.YourResources.Add(entity);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation($"Resource created with id: {entity.Id}");
        return entity;
    }
    catch (DbUpdateException ex)
    {
        _logger.LogError($"Database error adding resource: {ex.Message}");
        throw;
    }
}
```

#### Update Existing Entity
```csharp
public async Task<bool> UpdateAsync(YourResource entity)
{
    if (entity == null)
        throw new ArgumentNullException(nameof(entity));

    _logger.LogInformation($"Updating resource with id: {entity.Id}");
    
    try
    {
        var existingEntity = await _context.YourResources.FindAsync(entity.Id);
        if (existingEntity == null)
        {
            _logger.LogWarning($"Resource with id {entity.Id} not found.");
            return false;
        }

        entity.UpdatedAt = DateTime.UtcNow;
        
        _context.Entry(existingEntity).CurrentValues.SetValues(entity);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation($"Resource with id {entity.Id} updated successfully.");
        return true;
    }
    catch (DbUpdateException ex)
    {
        _logger.LogError($"Database error updating resource: {ex.Message}");
        throw;
    }
}
```

#### Update Specific Fields (Partial Update)
```csharp
public async Task<bool> UpdatePartialAsync(int id, Dictionary<string, object> updates)
{
    _logger.LogInformation($"Partially updating resource with id: {id}");
    
    var entity = await _context.YourResources.FindAsync(id);
    if (entity == null)
        return false;

    try
    {
        foreach (var update in updates)
        {
            var property = typeof(YourResource).GetProperty(update.Key);
            if (property != null && property.CanWrite)
            {
                property.SetValue(entity, update.Value);
            }
        }

        entity.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }
    catch (Exception ex)
    {
        _logger.LogError($"Error partially updating resource: {ex.Message}");
        throw;
    }
}
```

#### Delete Entity
```csharp
public async Task<bool> DeleteAsync(int id)
{
    _logger.LogInformation($"Deleting resource with id: {id}");
    
    try
    {
        var entity = await _context.YourResources.FindAsync(id);
        if (entity == null)
        {
            _logger.LogWarning($"Resource with id {id} not found for deletion.");
            return false;
        }

        _context.YourResources.Remove(entity);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation($"Resource with id {id} deleted successfully.");
        return true;
    }
    catch (DbUpdateException ex)
    {
        _logger.LogError($"Database error deleting resource: {ex.Message}");
        throw;
    }
}
```

#### Soft Delete (Mark as Deleted)
```csharp
public async Task<bool> SoftDeleteAsync(int id)
{
    _logger.LogInformation($"Soft deleting resource with id: {id}");
    
    var entity = await _context.YourResources.FindAsync(id);
    if (entity == null)
        return false;

    entity.IsDeleted = true;
    entity.DeletedAt = DateTime.UtcNow;
    
    await _context.SaveChangesAsync();
    return true;
}
```

#### Check Existence
```csharp
public async Task<bool> ExistsAsync(int id)
{
    return await _context.YourResources.AnyAsync(x => x.Id == id);
}

public async Task<bool> ExistsByNameAsync(string name)
{
    if (string.IsNullOrWhiteSpace(name))
        return false;
    
    return await _context.YourResources.AnyAsync(x => x.Name == name.Trim());
}
```

#### Get Count
```csharp
public async Task<int> GetCountAsync()
{
    return await _context.YourResources.CountAsync();
}

public async Task<int> GetCountByCategoryAsync(string category)
{
    return await _context.YourResources
        .Where(x => x.Category == category)
        .CountAsync();
}
```

#### Get with Related Data (Eager Loading)
```csharp
public async Task<YourResource> GetByIdWithDetailsAsync(int id)
{
    _logger.LogInformation($"Fetching resource with related data, id: {id}");
    
    var entity = await _context.YourResources
        .Include(x => x.RelatedItems)
        .Include(x => x.Category)
        .ThenInclude(x => x.ParentCategory)
        .FirstOrDefaultAsync(x => x.Id == id);
    
    return entity;
}
```

#### Batch Operations
```csharp
public async Task<IEnumerable<YourResource>> AddRangeAsync(IEnumerable<YourResource> entities)
{
    if (entities == null || !entities.Any())
        throw new ArgumentException("Entities collection cannot be null or empty.");

    _logger.LogInformation($"Adding {entities.Count()} resources in batch");
    
    try
    {
        foreach (var entity in entities)
        {
            entity.CreatedAt = DateTime.UtcNow;
            _context.YourResources.Add(entity);
        }
        
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Batch insert completed");
        
        return entities;
    }
    catch (Exception ex)
    {
        _logger.LogError($"Error in batch insert: {ex.Message}");
        throw;
    }
}

public async Task<bool> DeleteRangeAsync(IEnumerable<int> ids)
{
    _logger.LogInformation($"Deleting {ids.Count()} resources in batch");
    
    try
    {
        var entities = await _context.YourResources
            .Where(x => ids.Contains(x.Id))
            .ToListAsync();
        
        if (!entities.Any())
            return false;

        _context.YourResources.RemoveRange(entities);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation($"Batch delete completed");
        return true;
    }
    catch (Exception ex)
    {
        _logger.LogError($"Error in batch delete: {ex.Message}");
        throw;
    }
}
```

---

## Security Considerations

### 1. **SQL Injection Prevention**
- Always use parameterized queries (Entity Framework does this automatically)
- Never concatenate strings into queries
- Use LINQ methods for dynamic queries

```csharp
// SAFE - parameterized
var entity = await _context.YourResources
    .FirstOrDefaultAsync(x => x.Name == userInputName);

// UNSAFE - never do this
var query = $"SELECT * FROM YourResources WHERE Name = '{userInput}'";
```

### 2. **Data Access Control**
- Implement tenant/organization filtering in queries
- Filter by current user context for multi-tenant systems
- Never return data the user shouldn't access

```csharp
public async Task<YourResource> GetByIdAsync(int id, string userId)
{
    var entity = await _context.YourResources
        .FirstOrDefaultAsync(x => x.Id == id && x.OwnerId == userId);
    
    return entity;  // Returns null if user doesn't own it
}
```

### 3. **Sensitive Data Handling**
- Use `.Select()` to exclude sensitive columns from queries
- Hash passwords before storing - never expose in repository
- Implement column-level encryption for PII when needed

```csharp
public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
{
    // Only select non-sensitive data
    return await _context.Users
        .Select(x => new UserDto
        {
            Id = x.Id,
            Name = x.Name,
            Email = x.Email
            // Never include PasswordHash, TokenHash, etc.
        })
        .ToListAsync();
}
```

### 4. **Concurrency Control**
- Implement optimistic locking using RowVersion/Timestamp
- Handle DbUpdateConcurrencyException gracefully

```csharp
public async Task<bool> UpdateAsync(YourResource entity)
{
    try
    {
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return true;
    }
    catch (DbUpdateConcurrencyException ex)
    {
        _logger.LogError("Concurrency conflict: resource was modified by another user");
        throw;
    }
}
```

### 5. **Transaction Logging**
- Log all write operations (Create, Update, Delete)
- Include timestamp and user context
- Use audit tables for critical data changes

---

## Performance Optimization

### 1. **Query Optimization**
```csharp
// GOOD - selects only needed columns
public async Task<IEnumerable<CategorySummaryDto>> GetCategorySummaryAsync()
{
    return await _context.Categories
        .AsNoTracking()
        .Select(x => new CategorySummaryDto
        {
            Id = x.Id,
            Name = x.Name,
            Count = x.Items.Count()
        })
        .ToListAsync();
}

// BAD - loads entire entities then filters
public async Task<IEnumerable<Category>> GetActiveCategoriesAsync()
{
    var all = await _context.Categories.ToListAsync();
    return all.Where(x => x.IsActive).ToList();
}
```

### 2. **N+1 Query Problem Prevention**
```csharp
// GOOD - single query with joins
var categories = await _context.Categories
    .Include(x => x.Items)  // Eager load related items
    .Where(x => x.IsActive)
    .ToListAsync();

// BAD - causes N+1 queries
var categories = await _context.Categories.ToListAsync();
var items = categories.ForEach(async c => c.Items = await _context.Items
    .Where(i => i.CategoryId == c.Id).ToListAsync());
```

### 3. **Pagination Implementation**
```csharp
public async Task<(IEnumerable<YourResource> Items, int TotalCount)> GetPagedAsync(
    int pageNumber = 1, 
    int pageSize = 10)
{
    var query = _context.YourResources.AsQueryable();
    
    var totalCount = await query.CountAsync();
    
    var items = await query
        .OrderBy(x => x.CreatedAt)
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();
    
    return (items, totalCount);
}
```

### 4. **Using AsNoTracking for Read-Only Operations**
```csharp
// Better performance for read-only operations
var entities = await _context.YourResources
    .AsNoTracking()
    .Where(x => x.IsActive)
    .ToListAsync();
```

---

## Testing Recommendations

### Unit Testing Pattern
```csharp
[TestClass]
public class CategoryRepositoryTests
{
    private CategoryRepository _repository;
    private Mock<ProjectContext> _mockContext;

    [TestInitialize]
    public void Setup()
    {
        _mockContext = new Mock<ProjectContext>();
        _repository = new CategoryRepository(_mockContext.Object);
    }

    [TestMethod]
    public async Task GetByIdAsync_WithValidId_ReturnsCategory()
    {
        // Arrange
        var categories = new List<Category>
        {
            new Category { Id = 1, Name = "Electronics" }
        }.AsQueryable();

        var mockDbSet = new Mock<DbSet<Category>>();
        mockDbSet.As<IQueryable<Category>>().Setup(m => m.Provider).Returns(categories.Provider);
        mockDbSet.As<IQueryable<Category>>().Setup(m => m.Expression).Returns(categories.Expression);

        _mockContext.Setup(x => x.Categories).Returns(mockDbSet.Object);

        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Id);
        Assert.AreEqual("Electronics", result.Name);
    }
}
```

### Integration Testing with Real Database
- Use `InMemoryDatabase` for testing
- Create test fixtures with seed data
- Test actual DbContext interactions
- Clean up database between tests

---

## Common Patterns & Best Practices

### Repository Pattern with Specifications
```csharp
public interface ISpecification<T>
{
    IQueryable<T> Apply(IQueryable<T> query);
}

public async Task<IEnumerable<YourResource>> GetAsync(ISpecification<YourResource> spec)
{
    return await spec.Apply(_context.YourResources).ToListAsync();
}

// Usage
public class ActiveCategoriesSpecification : ISpecification<Category>
{
    public IQueryable<Category> Apply(IQueryable<Category> query)
    {
        return query.Where(x => x.IsActive).OrderBy(x => x.Name);
    }
}
```

### Unit of Work Pattern
```csharp
public interface IUnitOfWork
{
    ICategoryRepository Categories { get; }
    IProductRepository Products { get; }
    Task<int> SaveChangesAsync();
}

public class UnitOfWork : IUnitOfWork
{
    private readonly ProjectContext _context;
    
    public IcategoryRepository Categories { get; private set; }
    public IProductRepository Products { get; private set; }
    
    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
}
```

### Filtering/Sorting Helper
```csharp
public async Task<IEnumerable<YourResource>> GetFilteredAsync(
    string searchTerm,
    string sortField = "Name",
    bool ascending = true)
{
    IQueryable<YourResource> query = _context.YourResources;
    
    // Apply search filter
    if (!string.IsNullOrWhiteSpace(searchTerm))
    {
        query = query.Where(x => 
            x.Name.Contains(searchTerm) || 
            x.Description.Contains(searchTerm));
    }
    
    // Apply sorting
    query = (sortField.ToLower(), ascending) switch
    {
        ("name", true) => query.OrderBy(x => x.Name),
        ("name", false) => query.OrderByDescending(x => x.Name),
        ("createdat", true) => query.OrderBy(x => x.CreatedAt),
        ("createdat", false) => query.OrderByDescending(x => x.CreatedAt),
        _ => query.OrderBy(x => x.Id)
    };
    
    return await query.ToListAsync();
}
```

---

## Quick Checklist for New Repository Methods
- ✅ Use async/await for all database operations
- ✅ Use `.ToListAsync()`, `.FirstOrDefaultAsync()`, etc. instead of `.ToList()`, `.FirstOrDefault()`
- ✅ Implement pagination for GetAll methods
- ✅ Use `.Include()` for related data, `.AsNoTracking()` for read-only
- ✅ Handle null returns appropriately
- ✅ Add logging for all operations
- ✅ Use parameterized queries (Entity Framework does this)
- ✅ Return or throw meaningful exceptions
- ✅ Consider performance implications (N+1 queries, unnecessary data loading)
- ✅ Write integration tests with real database
- ✅ Validate parameters before using them
