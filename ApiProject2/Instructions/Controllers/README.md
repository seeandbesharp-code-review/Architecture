# Controllers Layer Instructions

## Purpose
Controllers are the entry point for HTTP requests in your API. They are responsible for:
- **Handling HTTP Requests**: Receiving incoming requests from clients (GET, POST, PUT, DELETE, etc.)
- **Request Validation**: Ensuring incoming data is valid before passing it to services
- **Routing**: Mapping URLs to appropriate action methods
- **Response Formatting**: Returning properly formatted HTTP responses with appropriate status codes
- **Error Handling**: Catching exceptions and returning meaningful error responses
- **Authorization/Authentication**: Enforcing access control (via attributes like `[Authorize]`)

Controllers act as a bridge between the client and business logic, keeping the API interface clean and secure.

---

## Where to Look
- **Location**: `ApiProject/Controllers/` folder
- **Example Files**: `AuthController.cs`, `CategoryController.cs`, `GiftsController.cs`, `CartController.cs`, etc.
- **Related DTO Files**: `ApiProject/DTO/` folder contains data transfer objects used in controller parameters and responses
- **Supporting Middleware**: `ApiProject/MiddleWare/` folder contains request/response handling logic

---

## Guidelines

### 1. **Naming Conventions**
- Controller classes should be named with the resource name followed by `Controller` (e.g., `CategoryController`, `GiftsController`)
- Action methods should use HTTP verb names when applicable: `Get`, `Post`, `Put`, `Delete`, `Patch`
- Use descriptive method names that clearly indicate the operation (e.g., `GetCategoryById`, `CreateProduct`)
- Route attributes should use kebab-case for URL segments (e.g., `[Route("api/categories")]`)

### 2. **REST API Principles**
- Use appropriate HTTP verbs:
  - **GET**: Retrieve resources (safe, idempotent, cacheable)
  - **POST**: Create new resources
  - **PUT**: Replace entire resources
  - **DELETE**: Remove resources
  - **PATCH**: Partial updates (when applicable)
- Return appropriate HTTP status codes:
  - `200 OK`: Successful GET, PUT, PATCH operations
  - `201 Created`: Successful POST operations with resource location
  - `204 No Content`: Successful DELETE operations
  - `400 Bad Request`: Invalid input data
  - `401 Unauthorized`: Missing/invalid authentication
  - `403 Forbidden`: Authenticated but not authorized
  - `404 Not Found`: Resource doesn't exist
  - `500 Internal Server Error`: Server errors

### 3. **Dependency Injection**
- Inject services via constructor using dependency injection
- Only inject what you need - follow the Interface Segregation Principle
- Use interfaces (e.g., `IServiceName`) for loose coupling and testability

### 4. **Request/Response Handling**
- Use DTOs for request/response data, not domain models directly
- Validate incoming requests early using model validation
- Map DTOs to domain models in the service layer
- Keep response bodies consistent and predictable
- Use the `[FromBody]`, `[FromRoute]`, `[FromQuery]` attributes for clarity

### 5. **Error Handling**
- Let middleware handle global exception handling when possible
- Catch specific exceptions and log them appropriately
- Return meaningful error messages to clients (without exposing sensitive information)
- Use `ModelState.IsValid` to check for validation errors

### 6. **Security**
- Use `[Authorize]` attribute to protect endpoints requiring authentication
- Use `[AllowAnonymous]` explicitly for public endpoints
- Validate all user inputs, even if model validation is in place
- Never expose sensitive information in error messages
- Use HTTPS in production
- Implement rate limiting for sensitive operations

### 7. **Async Operations**
- Use `async/await` for I/O-bound operations
- Method names should end with `Async` if they return `Task<T>` (convention)
- Always propagate `CancellationToken` when available

---

## Signature Conventions

### Basic Controller Structure
```csharp
using Microsoft.AspNetCore.Mvc;
using ApiProject.Services;
using ApiProject.DTO;

namespace ApiProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class YourResourceController : ControllerBase
    {
        private readonly IYourResourceService _service;

        public YourResourceController(IYourResourceService service)
        {
            _service = service;
        }

        // Action methods follow here
    }
}
```

### Standard Method Signatures

#### GET - Retrieve Single Item
```csharp
[HttpGet("{id}")]
public async Task<ActionResult<YourResourceDto>> GetById(int id)
{
    var result = await _service.GetByIdAsync(id);
    if (result == null)
        return NotFound($"Resource with id {id} not found.");
    return Ok(result);
}
```

#### GET - Retrieve All Items (with Pagination)
```csharp
[HttpGet]
public async Task<ActionResult<IEnumerable<YourResourceDto>>> GetAll(
    [FromQuery] int pageNumber = 1, 
    [FromQuery] int pageSize = 10)
{
    var results = await _service.GetAllAsync(pageNumber, pageSize);
    return Ok(results);
}
```

#### POST - Create New Item
```csharp
[HttpPost]
[Authorize]
public async Task<ActionResult<YourResourceDto>> Create([FromBody] CreateYourResourceDto dto)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState);

    var result = await _service.CreateAsync(dto);
    return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
}
```

#### PUT - Update Entire Item
```csharp
[HttpPut("{id}")]
[Authorize]
public async Task<IActionResult> Update(int id, [FromBody] UpdateYourResourceDto dto)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState);

    var result = await _service.UpdateAsync(id, dto);
    if (!result)
        return NotFound($"Resource with id {id} not found.");
    
    return NoContent();
}
```

#### DELETE - Remove Item
```csharp
[HttpDelete("{id}")]
[Authorize]
public async Task<IActionResult> Delete(int id)
{
    var result = await _service.DeleteAsync(id);
    if (!result)
        return NotFound($"Resource with id {id} not found.");
    
    return NoContent();
}
```

---

## Security Considerations

### 1. **Authentication & Authorization**
- Use `[Authorize]` attribute on sensitive endpoints
- Implement role-based access control with `[Authorize(Roles = "Admin")]` when applicable
- Validate user permissions in service layer for data-level access control

### 2. **Input Validation**
- Always validate incoming data using Data Annotations or FluentValidation
- Never trust client input
- Sanitize string inputs to prevent SQL injection
- Check file uploads for size, type, and content

### 3. **Data Exposure**
- Never return password hashes, personal information, or sensitive data
- Use DTOs to explicitly control what data is exposed
- Mask or redact sensitive information in error messages and logs

### 4. **CORS (Cross-Origin Resource Sharing)**
- Configure CORS policy in `Program.cs` explicitly
- Avoid using `AllowAnyOrigin` in production
- Specify allowed methods, headers, and credentials

### 5. **Rate Limiting**
- Implement rate limiting on endpoints that perform resource-intensive operations
- Apply stricter limits on authentication endpoints
- Log suspicious activity

### 6. **Logging & Monitoring**
- Log all authentication attempts and failures
- Log sensitive operations (modifications, deletions)
- Never log passwords or API keys
- Use appropriate log levels (Info, Warning, Error)

---

## Testing Recommendations

### Unit Testing
- Mock the service layer using frameworks like Moq
- Test successful operations and error scenarios
- Verify correct status codes are returned
- Test model validation

### Integration Testing
- Test actual API endpoints with a test database
- Test request/response serialization
- Test authentication and authorization
- Test error handling and edge cases

### Example Unit Test Pattern
```csharp
[TestClass]
public class CategoryControllerTests
{
    private Mock<ICategoryService> _mockService;
    private CategoryController _controller;

    [TestInitialize]
    public void Setup()
    {
        _mockService = new Mock<ICategoryService>();
        _controller = new CategoryController(_mockService.Object);
    }

    [TestMethod]
    public async Task GetById_WithValidId_ReturnsOkResult()
    {
        // Arrange
        var categoryId = 1;
        var categoryDto = new CategoryDto { Id = categoryId, Name = "Electronics" };
        _mockService.Setup(s => s.GetByIdAsync(categoryId))
            .ReturnsAsync(categoryDto);

        // Act
        var result = await _controller.GetById(categoryId);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        var okResult = (OkObjectResult)result.Result;
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.AreEqual(categoryDto, okResult.Value);
    }

    [TestMethod]
    public async Task GetById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var categoryId = 999;
        _mockService.Setup(s => s.GetByIdAsync(categoryId))
            .ReturnsAsync((CategoryDto)null);

        // Act
        var result = await _controller.GetById(categoryId);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
    }
}
```

---

## Common Patterns & Best Practices

### Error Response Format
Maintain consistency in error responses:
```csharp
public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; }
    public string Details { get; set; }
}
```

### Pagination Pattern
```csharp
public class PaginatedResponse<T>
{
    public List<T> Data { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public bool HasMore => (PageNumber * PageSize) < TotalCount;
}
```

### Using Custom Status Code Results
```csharp
// Return custom status code
return StatusCode(StatusCodes.Status201Created, result);
return StatusCode(StatusCodes.Status400BadRequest, errorMessage);
```

### Filtering & Sorting
```csharp
[HttpGet]
public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts(
    [FromQuery] string category,
    [FromQuery] string sortBy = "name",
    [FromQuery] bool ascending = true)
{
    var results = await _service.GetProductsAsync(category, sortBy, ascending);
    return Ok(results);
}
```

---

## Quick Checklist for New Controller Methods
- ✅ Use correct HTTP verb (GET, POST, PUT, DELETE)
- ✅ Return appropriate HTTP status code
- ✅ Use DTOs, not domain models
- ✅ Validate input data
- ✅ Inject required services via constructor
- ✅ Use `async/await` for I/O operations
- ✅ Add `[Authorize]` if needed
- ✅ Handle null/not found cases
- ✅ Log errors appropriately
- ✅ Write unit tests
