using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using LMS.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers;

/// <summary>Books, authors, categories, publishers and book copies. Any logged-in user can read; staff can write.</summary>
[Authorize]
public class CatalogController : ApiControllerBase
{
    private readonly ICatalogService _catalog;

    public CatalogController(ICatalogService catalog)
    {
        _catalog = catalog;
    }

    // ---------- Categories ----------

    [HttpGet("categories")]
    public async Task<ActionResult<PaginatedResult<CategoryDto>>> GetCategories([FromQuery] PaginationRequest request, [FromQuery] string? search, CancellationToken ct)
        => Ok(await _catalog.GetCategoriesAsync(request, search, ct));

    [HttpGet("categories/{id:guid}")]
    public async Task<ActionResult<CategoryDto>> GetCategoryById(Guid id, CancellationToken ct)
    {
        var item = await _catalog.GetCategoryByIdAsync(id, ct);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost("categories")]
    [Authorize(Policy = "LibrarianOrAdmin")]
    public async Task<ActionResult<CategoryDto>> CreateCategory([FromBody] CreateCategoryRequest request, CancellationToken ct)
    {
        var item = await _catalog.CreateCategoryAsync(request, ct);
        return CreatedAtAction(nameof(GetCategoryById), new { id = item.Id }, item);
    }

    [HttpPut("categories/{id:guid}")]
    [Authorize(Policy = "LibrarianOrAdmin")]
    public async Task<ActionResult<CategoryDto>> UpdateCategory(Guid id, [FromBody] UpdateCategoryRequest request, CancellationToken ct)
    {
        var item = await _catalog.UpdateCategoryAsync(id, request, ct);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpDelete("categories/{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> DeleteCategory(Guid id, CancellationToken ct)
        => await _catalog.DeleteCategoryAsync(id, ct) ? NoContent() : NotFound();

    // ---------- Publishers ----------

    [HttpGet("publishers")]
    public async Task<ActionResult<PaginatedResult<PublisherDto>>> GetPublishers([FromQuery] PaginationRequest request, [FromQuery] string? search, CancellationToken ct)
        => Ok(await _catalog.GetPublishersAsync(request, search, ct));

    [HttpGet("publishers/{id:guid}")]
    public async Task<ActionResult<PublisherDto>> GetPublisherById(Guid id, CancellationToken ct)
    {
        var item = await _catalog.GetPublisherByIdAsync(id, ct);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost("publishers")]
    [Authorize(Policy = "LibrarianOrAdmin")]
    public async Task<ActionResult<PublisherDto>> CreatePublisher([FromBody] CreatePublisherRequest request, CancellationToken ct)
    {
        var item = await _catalog.CreatePublisherAsync(request, ct);
        return CreatedAtAction(nameof(GetPublisherById), new { id = item.Id }, item);
    }

    [HttpPut("publishers/{id:guid}")]
    [Authorize(Policy = "LibrarianOrAdmin")]
    public async Task<ActionResult<PublisherDto>> UpdatePublisher(Guid id, [FromBody] UpdatePublisherRequest request, CancellationToken ct)
    {
        var item = await _catalog.UpdatePublisherAsync(id, request, ct);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpDelete("publishers/{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> DeletePublisher(Guid id, CancellationToken ct)
        => await _catalog.DeletePublisherAsync(id, ct) ? NoContent() : NotFound();

    // ---------- Authors ----------

    [HttpGet("authors")]
    public async Task<ActionResult<PaginatedResult<AuthorDto>>> GetAuthors([FromQuery] PaginationRequest request, [FromQuery] string? search, CancellationToken ct)
        => Ok(await _catalog.GetAuthorsAsync(request, search, ct));

    [HttpGet("authors/{id:guid}")]
    public async Task<ActionResult<AuthorDto>> GetAuthorById(Guid id, CancellationToken ct)
    {
        var item = await _catalog.GetAuthorByIdAsync(id, ct);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost("authors")]
    [Authorize(Policy = "LibrarianOrAdmin")]
    public async Task<ActionResult<AuthorDto>> CreateAuthor([FromBody] CreateAuthorRequest request, CancellationToken ct)
    {
        var item = await _catalog.CreateAuthorAsync(request, ct);
        return CreatedAtAction(nameof(GetAuthorById), new { id = item.Id }, item);
    }

    [HttpPut("authors/{id:guid}")]
    [Authorize(Policy = "LibrarianOrAdmin")]
    public async Task<ActionResult<AuthorDto>> UpdateAuthor(Guid id, [FromBody] UpdateAuthorRequest request, CancellationToken ct)
    {
        var item = await _catalog.UpdateAuthorAsync(id, request, ct);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpDelete("authors/{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> DeleteAuthor(Guid id, CancellationToken ct)
        => await _catalog.DeleteAuthorAsync(id, ct) ? NoContent() : NotFound();

    // ---------- Books ----------

    [HttpGet("books")]
    public async Task<ActionResult<PaginatedResult<BookDto>>> GetBooks([FromQuery] CatalogPaginationRequest request, [FromQuery] string? search, CancellationToken ct)
        => Ok(await _catalog.GetBooksAsync(request, request.CategoryId, request.PublisherId, search, ct));

    [HttpGet("books/search")]
    public async Task<ActionResult<IEnumerable<BookSummaryDto>>> SearchBooks([FromQuery] string q, [FromQuery] int maxResults = 10, CancellationToken ct = default)
        => Ok(await _catalog.SearchBooksAsync(q, maxResults, ct));

    [HttpGet("books/{id:guid}")]
    public async Task<ActionResult<BookDto>> GetBookById(Guid id, CancellationToken ct)
    {
        var book = await _catalog.GetBookByIdAsync(id, ct);
        return book == null ? NotFound() : Ok(book);
    }

    [HttpPost("books")]
    [Authorize(Policy = "LibrarianOrAdmin")]
    public async Task<ActionResult<BookDto>> CreateBook([FromBody] CreateBookRequest request, CancellationToken ct)
    {
        var book = await _catalog.CreateBookAsync(request, ct);
        return CreatedAtAction(nameof(GetBookById), new { id = book.Id }, book);
    }

    [HttpPut("books/{id:guid}")]
    [Authorize(Policy = "LibrarianOrAdmin")]
    public async Task<ActionResult<BookDto>> UpdateBook(Guid id, [FromBody] UpdateBookRequest request, CancellationToken ct)
    {
        var book = await _catalog.UpdateBookAsync(id, request, ct);
        return book == null ? NotFound() : Ok(book);
    }

    [HttpDelete("books/{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> DeleteBook(Guid id, CancellationToken ct)
        => await _catalog.DeleteBookAsync(id, ct) ? NoContent() : NotFound();

    // ---------- Book copies ----------

    [HttpGet("copies")]
    [Authorize(Policy = "LibrarianOrAdmin")]
    public async Task<ActionResult<PaginatedResult<BookCopyDto>>> GetCopies([FromQuery] PaginationRequest request, [FromQuery] Guid? bookId, [FromQuery] CopyStatus? status, CancellationToken ct)
        => Ok(await _catalog.GetBookCopiesAsync(request, bookId, status, ct));

    [HttpGet("copies/{id:guid}")]
    [Authorize(Policy = "LibrarianOrAdmin")]
    public async Task<ActionResult<BookCopyDto>> GetBookCopyById(Guid id, CancellationToken ct)
    {
        var copy = await _catalog.GetBookCopyByIdAsync(id, ct);
        return copy == null ? NotFound() : Ok(copy);
    }

    [HttpPost("copies")]
    [Authorize(Policy = "LibrarianOrAdmin")]
    public async Task<ActionResult<BookCopyDto>> CreateBookCopy([FromBody] CreateBookCopyRequest request, CancellationToken ct)
    {
        var copy = await _catalog.CreateBookCopyAsync(request, ct);
        return CreatedAtAction(nameof(GetBookCopyById), new { id = copy.Id }, copy);
    }

    [HttpPut("copies/{id:guid}")]
    [Authorize(Policy = "LibrarianOrAdmin")]
    public async Task<ActionResult<BookCopyDto>> UpdateBookCopy(Guid id, [FromBody] UpdateBookCopyRequest request, CancellationToken ct)
    {
        var copy = await _catalog.UpdateBookCopyAsync(id, request, ct);
        return copy == null ? NotFound() : Ok(copy);
    }

    [HttpDelete("copies/{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> DeleteBookCopy(Guid id, CancellationToken ct)
        => await _catalog.DeleteBookCopyAsync(id, ct) ? NoContent() : NotFound();
}
