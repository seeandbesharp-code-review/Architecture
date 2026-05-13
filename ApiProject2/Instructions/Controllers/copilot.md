# Controllers Layer - Quick Reference

## Purpose
Entry point for HTTP requests. Handles routing, request validation, response formatting, and error handling.

## Guidelines
- Use `[HttpGet]`, `[HttpPost]`, `[HttpPut]`, `[HttpDelete]` decorators
- Return appropriate HTTP status codes: 200 (OK), 201 (Created), 204 (No Content), 400 (Bad Request), 401 (Unauthorized), 404 (Not Found)
- Validate `ModelState.IsValid` before calling service
- Use DTOs for request/response, not domain models
- Inject services via constructor with dependency injection
- Use `async Task<ActionResult<T>>` for all methods
- Add `[Authorize]` for protected endpoints
- Catch exceptions from service layer and return appropriate error responses

## Security Notes
- Use `[Authorize]` attribute on sensitive endpoints
- Use `[AllowAnonymous]` explicitly for public endpoints
- Validate all user inputs (model validation + business rules)
- Never expose sensitive data in error messages
- Return `401 Unauthorized` for authentication failures, `403 Forbidden` for authorization failures

## Testing Recommendations
- Mock the service layer with Moq
- Test successful responses and error scenarios
- Verify correct HTTP status codes are returned
- Use `[TestMethod]` with arrange-act-assert pattern

## Code Examples

### Basic Controller Structure
```csharp
[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _service;

    public CategoriesController(ICategoryService service)
    {
        _service = service;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDto>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null)
            return NotFound(new { message = "Category not found" });
        return Ok(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<CategoryDto>> Create([FromBody] CreateCategoryDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        if (!result)
            return NotFound();
        return NoContent();
    }
}
```

### GET with Pagination
```csharp
[HttpGet]
public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll(
    [FromQuery] int pageNumber = 1, 
    [FromQuery] int pageSize = 10)
{
    var results = await _service.GetAllAsync(pageNumber, pageSize);
    return Ok(results);
}
```
