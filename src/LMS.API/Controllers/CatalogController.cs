using LMS.Application.DTOs.Catalog;
using LMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "LibrarianOrAdmin")]
[Produces("application/json")]
public class CatalogController : ControllerBase
{
    private readonly ICatalogService _catalogService;
    private readonly ILogger<CatalogController> _logger;

    public CatalogController(ICatalogService catalogService, ILogger<CatalogController> logger)
    {
        _catalogService = catalogService;
        _logger = logger;
    }

    #region Categories

    /// <summary>
    /// Get all categories with pagination
    /// </summary>
    [HttpGet("categories")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PagedResultDto<CategoryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResultDto<CategoryDto>>> GetCategories(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null)
    {
        var result = await _catalogService.GetCategoriesAsync(page, pageSize, search);
        return Ok(result);
    }

    /// <summary>
    /// Get category by ID
    /// </summary>
    [HttpGet("categories/{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryDto>> GetCategory(Guid id)
    {
        var category = await _catalogService.GetCategoryByIdAsync(id);
        if (category == null)
        {
            return NotFound(new { message = "Category not found" });
        }
        return Ok(category);
    }

    /// <summary>
    /// Create a new category
    /// </summary>
    [HttpPost("categories")]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CategoryDto>> CreateCategory([FromBody] CreateCategoryRequestDto request)
    {
        var category = await _catalogService.CreateCategoryAsync(request);
        return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
    }

    /// <summary>
    /// Update category
    /// </summary>
    [HttpPut("categories/{id:guid}")]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryDto>> UpdateCategory(Guid id, [FromBody] UpdateCategoryRequestDto request)
    {
        var category = await _catalogService.UpdateCategoryAsync(id, request);
        if (category == null)
        {
            return NotFound(new { message = "Category not found" });
        }
        return Ok(category);
    }

    /// <summary>
    /// Delete category
    /// </summary>
    [HttpDelete("categories/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCategory(Guid id)
    {
        var result = await _catalogService.DeleteCategoryAsync(id);
        if (!result)
        {
            return NotFound(new { message = "Category not found" });
        }
        return NoContent();
    }

    #endregion

    #region Publishers

    /// <summary>
    /// Get all publishers with pagination
    /// </summary>
    [HttpGet("publishers")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PagedResultDto<PublisherDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResultDto<PublisherDto>>> GetPublishers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null)
    {
        var result = await _catalogService.GetPublishersAsync(page, pageSize, search);
        return Ok(result);
    }

    /// <summary>
    /// Get publisher by ID
    /// </summary>
    [HttpGet("publishers/{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PublisherDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PublisherDto>> GetPublisher(Guid id)
    {
        var publisher = await _catalogService.GetPublisherByIdAsync(id);
        if (publisher == null)
        {
            return NotFound(new { message = "Publisher not found" });
        }
        return Ok(publisher);
    }

    /// <summary>
    /// Create a new publisher
    /// </summary>
    [HttpPost("publishers")]
    [ProducesResponseType(typeof(PublisherDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PublisherDto>> CreatePublisher([FromBody] CreatePublisherRequestDto request)
    {
        var publisher = await _catalogService.CreatePublisherAsync(request);
        return CreatedAtAction(nameof(GetPublisher), new { id = publisher.Id }, publisher);
    }

    /// <summary>
    /// Update publisher
    /// </summary>
    [HttpPut("publishers/{id:guid}")]
    [ProducesResponseType(typeof(PublisherDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PublisherDto>> UpdatePublisher(Guid id, [FromBody] UpdatePublisherRequestDto request)
    {
        var publisher = await _catalogService.UpdatePublisherAsync(id, request);
        if (publisher == null)
        {
            return NotFound(new { message = "Publisher not found" });
        }
        return Ok(publisher);
    }

    /// <summary>
    /// Delete publisher
    /// </summary>
    [HttpDelete("publishers/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePublisher(Guid id)
    {
        var result = await _catalogService.DeletePublisherAsync(id);
        if (!result)
        {
            return NotFound(new { message = "Publisher not found" });
        }
        return NoContent();
    }

    #endregion

    #region Authors

    /// <summary>
    /// Get all authors with pagination
    /// </summary>
    [HttpGet("authors")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PagedResultDto<AuthorDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResultDto<AuthorDto>>> GetAuthors(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null)
    {
        var result = await _catalogService.GetAuthorsAsync(page, pageSize, search);
        return Ok(result);
    }

    /// <summary>
    /// Get author by ID
    /// </summary>
    [HttpGet("authors/{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AuthorDto>> GetAuthor(Guid id)
    {
        var author = await _catalogService.GetAuthorByIdAsync(id);
        if (author == null)
        {
            return NotFound(new { message = "Author not found" });
        }
        return Ok(author);
    }

    /// <summary>
    /// Create a new author
    /// </summary>
    [HttpPost("authors")]
    [ProducesResponseType(typeof(AuthorDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthorDto>> CreateAuthor([FromBody] CreateAuthorRequestDto request)
    {
        var author = await _catalogService.CreateAuthorAsync(request);
        return CreatedAtAction(nameof(GetAuthor), new { id = author.Id }, author);
    }

    /// <summary>
    /// Update author
    /// </summary>
    [HttpPut("authors/{id:guid}")]
    [ProducesResponseType(typeof(AuthorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AuthorDto>> UpdateAuthor(Guid id, [FromBody] UpdateAuthorRequestDto request)
    {
        var author = await _catalogService.UpdateAuthorAsync(id, request);
        if (author == null)
        {
            return NotFound(new { message = "Author not found" });
        }
        return Ok(author);
    }

    /// <summary>
    /// Delete author
    /// </summary>
    [HttpDelete("authors/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAuthor(Guid id)
    {
        var result = await _catalogService.DeleteAuthorAsync(id);
        if (!result)
        {
            return NotFound(new { message = "Author not found" });
        }
        return NoContent();
    }

    #endregion

    #region Books

    /// <summary>
    /// Get all books with pagination and filters
    /// </summary>
    [HttpGet("books")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PagedResultDto<BookDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResultDto<BookDto>>> GetBooks(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] Guid? categoryId = null,
        [FromQuery] Guid? publisherId = null,
        [FromQuery] Guid? authorId = null,
        [FromQuery] bool? isAvailable = null)
    {
        var result = await _catalogService.GetBooksAsync(page, pageSize, search, categoryId, publisherId, authorId, isAvailable);
        return Ok(result);
    }

    /// <summary>
    /// Get book by ID
    /// </summary>
    [HttpGet("books/{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(BookDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookDto>> GetBook(Guid id)
    {
        var book = await _catalogService.GetBookByIdAsync(id);
        if (book == null)
        {
            return NotFound(new { message = "Book not found" });
        }
        return Ok(book);
    }

    /// <summary>
    /// Create a new book
    /// </summary>
    [HttpPost("books")]
    [ProducesResponseType(typeof(BookDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BookDto>> CreateBook([FromBody] CreateBookRequestDto request)
    {
        var book = await _catalogService.CreateBookAsync(request);
        return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
    }

    /// <summary>
    /// Update book
    /// </summary>
    [HttpPut("books/{id:guid}")]
    [ProducesResponseType(typeof(BookDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookDto>> UpdateBook(Guid id, [FromBody] UpdateBookRequestDto request)
    {
        var book = await _catalogService.UpdateBookAsync(id, request);
        if (book == null)
        {
            return NotFound(new { message = "Book not found" });
        }
        return Ok(book);
    }

    /// <summary>
    /// Delete book
    /// </summary>
    [HttpDelete("books/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteBook(Guid id)
    {
        var result = await _catalogService.DeleteBookAsync(id);
        if (!result)
        {
            return NotFound(new { message = "Book not found" });
        }
        return NoContent();
    }

    /// <summary>
    /// Add author to book
    /// </summary>
    [HttpPost("books/{bookId:guid}/authors")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddAuthorToBook(Guid bookId, [FromBody] AddAuthorToBookRequestDto request)
    {
        var result = await _catalogService.AddAuthorToBookAsync(bookId, request.AuthorId);
        if (!result)
        {
            return BadRequest(new { message = "Failed to add author. Book or author not found." });
        }
        return Ok(new { message = "Author added to book successfully" });
    }

    /// <summary>
    /// Remove author from book
    /// </summary>
    [HttpDelete("books/{bookId:guid}/authors/{authorId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveAuthorFromBook(Guid bookId, Guid authorId)
    {
        var result = await _catalogService.RemoveAuthorFromBookAsync(bookId, authorId);
        if (!result)
        {
            return NotFound(new { message = "Book or author not found" });
        }
        return Ok(new { message = "Author removed from book successfully" });
    }

    #endregion

    #region Book Copies

    /// <summary>
    /// Get all book copies with pagination
    /// </summary>
    [HttpGet("books/{bookId:guid}/copies")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PagedResultDto<BookCopyDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResultDto<BookCopyDto>>> GetBookCopies(
        Guid bookId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? status = null)
    {
        var result = await _catalogService.GetBookCopiesAsync(bookId, page, pageSize, status);
        return Ok(result);
    }

    /// <summary>
    /// Get book copy by ID
    /// </summary>
    [HttpGet("copies/{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(BookCopyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookCopyDto>> GetBookCopy(Guid id)
    {
        var copy = await _catalogService.GetBookCopyByIdAsync(id);
        if (copy == null)
        {
            return NotFound(new { message = "Book copy not found" });
        }
        return Ok(copy);
    }

    /// <summary>
    /// Create a new book copy
    /// </summary>
    [HttpPost("books/{bookId:guid}/copies")]
    [ProducesResponseType(typeof(BookCopyDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BookCopyDto>> CreateBookCopy(Guid bookId, [FromBody] CreateBookCopyRequestDto request)
    {
        var copy = await _catalogService.CreateBookCopyAsync(bookId, request);
        return CreatedAtAction(nameof(GetBookCopy), new { id = copy.Id }, copy);
    }

    /// <summary>
    /// Update book copy
    /// </summary>
    [HttpPut("copies/{id:guid}")]
    [ProducesResponseType(typeof(BookCopyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookCopyDto>> UpdateBookCopy(Guid id, [FromBody] UpdateBookCopyRequestDto request)
    {
        var copy = await _catalogService.UpdateBookCopyAsync(id, request);
        if (copy == null)
        {
            return NotFound(new { message = "Book copy not found" });
        }
        return Ok(copy);
    }

    /// <summary>
    /// Delete book copy
    /// </summary>
    [HttpDelete("copies/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteBookCopy(Guid id)
    {
        var result = await _catalogService.DeleteBookCopyAsync(id);
        if (!result)
        {
            return NotFound(new { message = "Book copy not found" });
        }
        return NoContent();
    }

    #endregion
}