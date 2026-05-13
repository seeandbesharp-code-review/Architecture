# Services Layer Instructions

## Purpose
The Service layer contains the business logic of your application. It is responsible for:
- **Business Logic Implementation**: Implementing core business rules and workflows
- **Data Validation**: Validating business rules (beyond data type validation)
- **Orchestration**: Coordinating operations across multiple repositories or external services
- **Data Transformation**: Converting between DTOs and domain models
- **Transaction Management**: Managing complex multi-step operations as atomic units
- **Error Handling**: Catching and handling repository/external service errors gracefully
- **Caching Strategy**: Implementing caching logic when applicable
- **External Service Integration**: Handling calls to third-party services, APIs, or email providers

Services are the heart of your application logic, independent of how data is stored or how it's presented to clients.

---

## Where to Look
- **Location**: `ApiProject/Services/` folder (create interfaces and implementations here)
- **Related Repositories**: `ApiProject/Repositories/` folder (called by services for data access)
- **Related DTOs**: `ApiProject/DTO/` folder (services work with these for data transfer)
- **Related Models**: `ApiProject/Models/` folder (domain models used internally)
- **Data Context**: `ApiProject/Data/ProjectContext.cs` (passed to repositories)

---

## Guidelines

### 1. **Naming Conventions**
- Interface names should be prefixed with `I` (e.g., `ICategoryService`, `IProductService`)
- Implementation class names should match the interface without `I` (e.g., `CategoryService`, `ProductService`)
- Method names should use `async` suffix if returning `Task<T>` (e.g., `GetByIdAsync`, `CreateAsync`)
- Method names should be descriptive of the business operation (e.g., `ApplyDiscount`, `CalculateTotal`, `ProcessRefund`)

### 2. **Separation of Concerns**
- Keep services focused on a single domain entity or related functionality
- Don't mix UI logic with business logic
- Don't include database access code directly - use repositories
- Delegate cross-cutting concerns (logging, caching) to appropriate layers

### 3. **Dependency Injection**
- Inject repository interfaces via constructor
- Inject other services only when needed for business orchestration
- Use interfaces, not concrete implementations (for testability)
- Make injected dependencies `readonly` to prevent accidental modification

### 4. **Data Transformation**
- Use mapping libraries (e.g., AutoMapper) or manual mapping for DTO ↔ Model conversions
- Keep mapping logic in the service or use a dedicated mapper class
- Never pass domain models directly to controllers - always use DTOs
- Validate data before mapping to domain models

### 5. **Error Handling**
- Catch repository exceptions and translate them to business exceptions
- Log exceptions with sufficient context for debugging
- Throw meaningful custom exceptions that controllers can catch
- Don't let database exceptions bubble up to the controller

### 6. **Async Operations**
- Use `async/await` throughout the service layer
- Propagate `CancellationToken` from controllers for graceful cancellation
- All repository method calls should be awaited

### 7. **Business Rule Validation**
- Validate complex business rules in the service layer
- Examples: checking stock availability, validating purchase limits, verifying user permissions
- Return meaningful error messages or throw custom exceptions

---

## Signature Conventions

### Basic Service Interface & Implementation

```csharp
using ApiProject.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiProject.Services
{
    public interface IYourResourceService
    {
        // Read operations
        Task<YourResourceDto> GetByIdAsync(int id);
        Task<IEnumerable<YourResourceDto>> GetAllAsync(int pageNumber, int pageSize);
        
        // Write operations
        Task<YourResourceDto> CreateAsync(CreateYourResourceDto dto);
        Task<bool> UpdateAsync(int id, UpdateYourResourceDto dto);
        Task<bool> DeleteAsync(int id);
    }

    public class YourResourceService : IYourResourceService
    {
        private readonly IYourResourceRepository _repository;
        private readonly ILogger<YourResourceService> _logger;

        public YourResourceService(IYourResourceRepository repository, ILogger<YourResourceService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        // Implementation methods follow
    }
}
```

### Standard Method Patterns

#### Get Single Item with Error Handling
```csharp
public async Task<YourResourceDto> GetByIdAsync(int id)
{
    try
    {
        _logger.LogInformation($"Retrieving resource with id: {id}");
        
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
        {
            _logger.LogWarning($"Resource with id {id} not found.");
            throw new KeyNotFoundException($"Resource with id {id} does not exist.");
        }

        return MapToDto(entity);
    }
    catch (Exception ex)
    {
        _logger.LogError($"Error retrieving resource: {ex.Message}");
        throw;
    }
}
```

#### Get All Items with Pagination
```csharp
public async Task<IEnumerable<YourResourceDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
{
    _logger.LogInformation($"Retrieving all resources - Page: {pageNumber}, Size: {pageSize}");
    
    if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
        throw new ArgumentException("Invalid pagination parameters.");

    var entities = await _repository.GetAllAsync(pageNumber, pageSize);
    return entities.Select(MapToDto).ToList();
}
```

#### Create with Business Logic Validation
```csharp
public async Task<YourResourceDto> CreateAsync(CreateYourResourceDto dto)
{
    // Validate business rules
    var existingResource = await _repository.GetByNameAsync(dto.Name);
    if (existingResource != null)
        throw new InvalidOperationException($"Resource with name '{dto.Name}' already exists.");

    if (string.IsNullOrWhiteSpace(dto.Name))
        throw new ArgumentException("Resource name cannot be empty.");

    try
    {
        _logger.LogInformation($"Creating new resource: {dto.Name}");
        
        // Map DTO to domain model
        var entity = new YourResource
        {
            Name = dto.Name,
            Description = dto.Description,
            CreatedAt = DateTime.UtcNow
        };

        var createdEntity = await _repository.CreateAsync(entity);
        _logger.LogInformation($"Resource created with id: {createdEntity.Id}");
        
        return MapToDto(createdEntity);
    }
    catch (Exception ex)
    {
        _logger.LogError($"Error creating resource: {ex.Message}");
        throw;
    }
}
```

#### Update with Partial Business Logic
```csharp
public async Task<bool> UpdateAsync(int id, UpdateYourResourceDto dto)
{
    try
    {
        _logger.LogInformation($"Updating resource with id: {id}");
        
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
        {
            _logger.LogWarning($"Resource with id {id} not found for update.");
            return false;
        }

        // Validate business rules before update
        if (!string.IsNullOrWhiteSpace(dto.Name) && dto.Name != entity.Name)
        {
            var duplicate = await _repository.GetByNameAsync(dto.Name);
            if (duplicate != null)
                throw new InvalidOperationException($"Another resource with name '{dto.Name}' already exists.");
        }

        // Update only provided fields
        if (!string.IsNullOrWhiteSpace(dto.Name))
            entity.Name = dto.Name;
        if (!string.IsNullOrWhiteSpace(dto.Description))
            entity.Description = dto.Description;
        
        entity.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(entity);
        _logger.LogInformation($"Resource with id {id} updated successfully.");
        
        return true;
    }
    catch (Exception ex)
    {
        _logger.LogError($"Error updating resource: {ex.Message}");
        throw;
    }
}
```

#### Delete with Cascade Checks
```csharp
public async Task<bool> DeleteAsync(int id)
{
    try
    {
        _logger.LogInformation($"Deleting resource with id: {id}");
        
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
        {
            _logger.LogWarning($"Resource with id {id} not found for deletion.");
            return false;
        }

        // Check for dependencies before deletion
        var relatedItems = await _repository.GetRelatedItemsAsync(id);
        if (relatedItems.Any())
            throw new InvalidOperationException($"Cannot delete resource with {relatedItems.Count()} related items.");

        await _repository.DeleteAsync(id);
        _logger.LogInformation($"Resource with id {id} deleted successfully.");
        
        return true;
    }
    catch (Exception ex)
    {
        _logger.LogError($"Error deleting resource: {ex.Message}");
        throw;
    }
}
```

#### Complex Business Operation (Orchestration)
```csharp
public async Task<OrderProcessingResultDto> ProcessOrderAsync(int orderId, CancellationToken cancellationToken = default)
{
    try
    {
        _logger.LogInformation($"Processing order {orderId}");

        // Step 1: Retrieve order
        var order = await _repository.GetOrderByIdAsync(orderId, cancellationToken);
        if (order == null)
            throw new KeyNotFoundException($"Order {orderId} not found.");

        // Step 2: Validate inventory
        var inventoryValid = await ValidateInventoryAsync(order, cancellationToken);
        if (!inventoryValid)
            throw new InvalidOperationException("Insufficient inventory to process order.");

        // Step 3: Process payment
        var paymentResult = await _paymentService.ProcessPaymentAsync(order, cancellationToken);
        if (!paymentResult.Success)
            throw new InvalidOperationException($"Payment failed: {paymentResult.Message}");

        // Step 4: Update order status
        order.Status = OrderStatus.Paid;
        order.ProcessedAt = DateTime.UtcNow;
        await _repository.UpdateOrderAsync(order, cancellationToken);

        // Step 5: Send confirmation
        await _emailService.SendOrderConfirmationAsync(order.CustomerId, order, cancellationToken);

        _logger.LogInformation($"Order {orderId} processed successfully.");
        
        return new OrderProcessingResultDto
        {
            Success = true,
            OrderId = orderId,
            Message = "Order processed successfully."
        };
    }
    catch (Exception ex)
    {
        _logger.LogError($"Error processing order {orderId}: {ex.Message}");
        throw;
    }
}
```

#### DTO to Model Mapping
```csharp
private YourResourceDto MapToDto(YourResource entity)
{
    return new YourResourceDto
    {
        Id = entity.Id,
        Name = entity.Name,
        Description = entity.Description,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt
    };
}

private YourResource MapToDomain(CreateYourResourceDto dto)
{
    return new YourResource
    {
        Name = dto.Name,
        Description = dto.Description
    };
}
```

---

## Security Considerations

### 1. **Authorization at Service Level**
- Implement permission checks in service methods
- Verify user has permission to perform operation on the resource
- Use a service like `IAuthorizationService` for complex permission logic

```csharp
public async Task<YourResourceDto> GetByIdAsync(int id, string userId)
{
    var entity = await _repository.GetByIdAsync(id);
    if (entity == null)
        throw new KeyNotFoundException("Resource not found.");

    // Check authorization
    if (entity.OwnerId != userId && !userIsAdmin)
        throw new UnauthorizedAccessException("You don't have permission to access this resource.");

    return MapToDto(entity);
}
```

### 2. **Data Sanitization**
- Sanitize string inputs that will be used in queries
- Validate and clean data before persisting
- Use parameterized queries in repositories (Entity Framework does this automatically)

### 3. **Sensitive Data Handling**
- Don't log passwords, tokens, or personal information
- Hash sensitive data before storing
- Mask sensitive data in error messages
- Use DTOs to control what data is exposed

### 4. **Concurrency Handling**
- Implement optimistic locking for concurrent updates
- Use version numbers or timestamps to detect conflicts
- Return appropriate error messages for conflict scenarios

### 5. **Transaction Management**
- Ensure atomic operations for multi-step business processes
- Implement rollback on failure
- Use database transactions for consistency

```csharp
public async Task<bool> TransferCreditsAsync(int fromUserId, int toUserId, decimal amount)
{
    using (var transaction = await _context.Database.BeginTransactionAsync())
    {
        try
        {
            // Deduct from source
            var source = await _userRepository.GetByIdAsync(fromUserId);
            if (source.Credits < amount)
                throw new InvalidOperationException("Insufficient credits.");
            
            source.Credits -= amount;
            await _userRepository.UpdateAsync(source);

            // Add to destination
            var destination = await _userRepository.GetByIdAsync(toUserId);
            destination.Credits += amount;
            await _userRepository.UpdateAsync(destination);

            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
```

---

## Testing Recommendations

### Unit Testing Pattern
```csharp
[TestClass]
public class CategoryServiceTests
{
    private Mock<ICategoryRepository> _mockRepository;
    private CategoryService _service;

    [TestInitialize]
    public void Setup()
    {
        _mockRepository = new Mock<ICategoryRepository>();
        _service = new CategoryService(_mockRepository.Object);
    }

    [TestMethod]
    public async Task CreateAsync_WithValidDto_ReturnsCreatedDto()
    {
        // Arrange
        var createDto = new CreateCategoryDto { Name = "Electronics" };
        var createdEntity = new Category { Id = 1, Name = "Electronics", CreatedAt = DateTime.UtcNow };
        
        _mockRepository.Setup(r => r.GetByNameAsync("Electronics"))
            .ReturnsAsync((Category)null);
        _mockRepository.Setup(r => r.CreateAsync(It.IsAny<Category>()))
            .ReturnsAsync(createdEntity);

        // Act
        var result = await _service.CreateAsync(createDto);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Id);
        Assert.AreEqual("Electronics", result.Name);
        _mockRepository.Verify(r => r.CreateAsync(It.IsAny<Category>()), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public async Task CreateAsync_WithDuplicateName_ThrowsException()
    {
        // Arrange
        var createDto = new CreateCategoryDto { Name = "Electronics" };
        var existingCategory = new Category { Id = 1, Name = "Electronics" };
        
        _mockRepository.Setup(r => r.GetByNameAsync("Electronics"))
            .ReturnsAsync(existingCategory);

        // Act
        await _service.CreateAsync(createDto);
        // Assert - expect exception
    }

    [TestMethod]
    public async Task GetByIdAsync_WithValidId_ReturnsDto()
    {
        // Arrange
        var category = new Category { Id = 1, Name = "Electronics" };
        _mockRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(category);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Id);
        Assert.AreEqual("Electronics", result.Name);
    }

    [TestMethod]
    [ExpectedException(typeof(KeyNotFoundException))]
    public async Task GetByIdAsync_WithInvalidId_ThrowsKeyNotFoundException()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((Category)null);

        // Act
        await _service.GetByIdAsync(999);
        // Assert - expect exception
    }
}
```

### Integration Testing
- Test services with actual database (use test database)
- Test external service integrations with mocks/stubs
- Test end-to-end workflows
- Test error scenarios and exception handling

---

## Common Patterns & Best Practices

### Result Pattern for Operations
```csharp
public class ServiceResult<T>
{
    public bool Success { get; set; }
    public T Data { get; set; }
    public string Message { get; set; }
    public List<string> Errors { get; set; } = new();
}

// Usage
public async Task<ServiceResult<YourResourceDto>> CreateAsync(CreateYourResourceDto dto)
{
    var result = new ServiceResult<YourResourceDto>();
    
    try
    {
        // Implementation
        result.Success = true;
        result.Data = createdDto;
        result.Message = "Created successfully.";
    }
    catch (Exception ex)
    {
        result.Success = false;
        result.Errors.Add(ex.Message);
    }
    
    return result;
}
```

### Validation Helper
```csharp
private void ValidateCreateDto(CreateYourResourceDto dto)
{
    var errors = new List<string>();
    
    if (string.IsNullOrWhiteSpace(dto.Name))
        errors.Add("Name is required.");
    
    if (dto.Name?.Length > 100)
        errors.Add("Name cannot exceed 100 characters.");
    
    if (errors.Any())
        throw new ValidationException(string.Join(", ", errors));
}
```

### Caching Pattern
```csharp
private const string CACHE_KEY = "AllCategories";

public async Task<IEnumerable<CategoryDto>> GetAllAsync()
{
    var cached = await _cache.GetAsync(CACHE_KEY);
    if (cached != null)
        return JsonConvert.DeserializeObject<IEnumerable<CategoryDto>>(cached);

    var entities = await _repository.GetAllAsync();
    var dtos = entities.Select(MapToDto).ToList();
    
    await _cache.SetAsync(CACHE_KEY, JsonConvert.SerializeObject(dtos), TimeSpan.FromHours(1));
    return dtos;
}

public async Task InvalidateCacheAsync()
{
    await _cache.RemoveAsync(CACHE_KEY);
}
```

---

## Quick Checklist for New Service Methods
- ✅ Use async/await for any I/O operations
- ✅ Implement business logic validation
- ✅ Map between DTOs and domain models
- ✅ Include error handling with logging
- ✅ Check authorization/permissions
- ✅ Return or throw meaningful exceptions
- ✅ Use dependency injection for repositories
- ✅ Write unit tests with mocked repositories
- ✅ Document complex business logic
- ✅ Propagate CancellationToken when applicable
