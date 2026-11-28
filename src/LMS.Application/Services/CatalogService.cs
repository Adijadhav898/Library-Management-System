using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using LMS.Domain.Entities;
using LMS.Domain.Enums;

namespace LMS.Application.Services;

public class CatalogService : ICatalogService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IPublisherRepository _publisherRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IBookCopyRepository _bookCopyRepository;

    public CatalogService(
        ICategoryRepository categoryRepository,
        IPublisherRepository publisherRepository,
        IAuthorRepository authorRepository,
        IBookRepository bookRepository,
        IBookCopyRepository bookCopyRepository)
    {
        _categoryRepository = categoryRepository;
        _publisherRepository = publisherRepository;
        _authorRepository = authorRepository;
        _bookRepository = bookRepository;
        _bookCopyRepository = bookCopyRepository;
    }

    // Category methods
    public async Task<PaginatedResult<CategoryDto>> GetCategoriesAsync(PaginationRequest request, string? search = null, CancellationToken cancellationToken = default)
    {
        var (categories, totalCount) = await _categoryRepository.GetPagedAsync(request, search, cancellationToken);
        return new PaginatedResult<CategoryDto>
        {
            Items = categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                ParentCategoryId = c.ParentCategoryId,
                ParentCategoryName = c.ParentCategory?.Name,
                DisplayOrder = c.DisplayOrder,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt
            }),
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<PaginatedResult<CategoryDto>> GetCategoriesPagedAsync(PaginationRequest request, string? search = null, CancellationToken cancellationToken = default)
    {
        return await GetCategoriesAsync(request, search, cancellationToken);
    }

    public async Task<CategoryDto?> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        if (category == null)
            return null;

        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            ParentCategoryId = category.ParentCategoryId,
            ParentCategoryName = category.ParentCategory?.Name,
            DisplayOrder = category.DisplayOrder,
            IsActive = category.IsActive,
            CreatedAt = category.CreatedAt
        };
    }

    public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            ParentCategoryId = request.ParentCategoryId,
            DisplayOrder = request.DisplayOrder,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _categoryRepository.AddAsync(category, cancellationToken);
        return await GetCategoryByIdAsync(category.Id, cancellationToken) ?? throw new InvalidOperationException("Failed to create category");
    }

    public async Task<CategoryDto?> UpdateCategoryAsync(Guid id, UpdateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        if (category == null)
            return null;

        category.Name = request.Name;
        category.Description = request.Description;
        category.ParentCategoryId = request.ParentCategoryId;
        category.DisplayOrder = request.DisplayOrder;
        category.IsActive = request.IsActive;
        category.UpdatedAt = DateTime.UtcNow;

        await _categoryRepository.UpdateAsync(category, cancellationToken);
        return await GetCategoryByIdAsync(id, cancellationToken);
    }

    public async Task<bool> DeleteCategoryAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        if (category == null)
            return false;

        category.IsDeleted = true;
        category.UpdatedAt = DateTime.UtcNow;
        await _categoryRepository.UpdateAsync(category, cancellationToken);
        return true;
    }

    // Publisher methods
    public async Task<PaginatedResult<PublisherDto>> GetPublishersAsync(PaginationRequest request, string? search = null, CancellationToken cancellationToken = default)
    {
        var (publishers, totalCount) = await _publisherRepository.GetPagedAsync(request, search, cancellationToken);

        return new PaginatedResult<PublisherDto>
        {
            Items = publishers.Select(p => new PublisherDto
            {
                Id = p.Id,
                Name = p.Name,
                Address = p.Address,
                Phone = p.Phone,
                Email = p.Email,
                Website = p.Website,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedAt
            }),
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<PublisherDto?> GetPublisherByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var publisher = await _publisherRepository.GetByIdAsync(id, cancellationToken);
        if (publisher == null)
            return null;

        return new PublisherDto
        {
            Id = publisher.Id,
            Name = publisher.Name,
            Address = publisher.Address,
            Phone = publisher.Phone,
            Email = publisher.Email,
            Website = publisher.Website,
            IsActive = publisher.IsActive,
            CreatedAt = publisher.CreatedAt
        };
    }

    public async Task<PublisherDto> CreatePublisherAsync(CreatePublisherRequest request, CancellationToken cancellationToken = default)
    {
        var publisher = new Publisher
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Address = request.Address,
            Phone = request.Phone,
            Email = request.Email,
            Website = request.Website,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _publisherRepository.AddAsync(publisher, cancellationToken);
        return await GetPublisherByIdAsync(publisher.Id, cancellationToken) ?? throw new InvalidOperationException("Failed to create publisher");
    }

    public async Task<PublisherDto?> UpdatePublisherAsync(Guid id, UpdatePublisherRequest request, CancellationToken cancellationToken = default)
    {
        var publisher = await _publisherRepository.GetByIdAsync(id, cancellationToken);
        if (publisher == null)
            return null;

        publisher.Name = request.Name;
        publisher.Address = request.Address;
        publisher.Phone = request.Phone;
        publisher.Email = request.Email;
        publisher.Website = request.Website;
        publisher.IsActive = request.IsActive;
        publisher.UpdatedAt = DateTime.UtcNow;

        await _publisherRepository.UpdateAsync(publisher, cancellationToken);
        return await GetPublisherByIdAsync(id, cancellationToken);
    }

    public async Task<bool> DeletePublisherAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var publisher = await _publisherRepository.GetByIdAsync(id, cancellationToken);
        if (publisher == null)
            return false;

        publisher.IsDeleted = true;
        publisher.UpdatedAt = DateTime.UtcNow;
        await _publisherRepository.UpdateAsync(publisher, cancellationToken);
        return true;
    }

    // Author methods
    public async Task<PaginatedResult<AuthorDto>> GetAuthorsAsync(PaginationRequest request, string? search = null, CancellationToken cancellationToken = default)
    {
        var (authors, totalCount) = await _authorRepository.GetPagedAsync(request, search, cancellationToken);

        return new PaginatedResult<AuthorDto>
        {
            Items = authors.Select(a => new AuthorDto
            {
                Id = a.Id,
                FirstName = a.FirstName,
                LastName = a.LastName,
                Biography = a.Biography,
                DateOfBirth = a.DateOfBirth,
                DateOfDeath = a.DateOfDeath,
                Nationality = a.Nationality,
                CreatedAt = a.CreatedAt
            }),
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<AuthorDto?> GetAuthorByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var author = await _authorRepository.GetByIdAsync(id, cancellationToken);
        if (author == null)
            return null;

        return new AuthorDto
        {
            Id = author.Id,
            FirstName = author.FirstName,
            LastName = author.LastName,
            Biography = author.Biography,
            DateOfBirth = author.DateOfBirth,
            DateOfDeath = author.DateOfDeath,
            Nationality = author.Nationality,
            CreatedAt = author.CreatedAt
        };
    }

    public async Task<AuthorDto> CreateAuthorAsync(CreateAuthorRequest request, CancellationToken cancellationToken = default)
    {
        var author = new Author
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Biography = request.Biography,
            DateOfBirth = request.DateOfBirth,
            DateOfDeath = request.DateOfDeath,
            Nationality = request.Nationality,
            CreatedAt = DateTime.UtcNow
        };

        await _authorRepository.AddAsync(author, cancellationToken);
        return await GetAuthorByIdAsync(author.Id, cancellationToken) ?? throw new InvalidOperationException("Failed to create author");
    }

    public async Task<AuthorDto?> UpdateAuthorAsync(Guid id, UpdateAuthorRequest request, CancellationToken cancellationToken = default)
    {
        var author = await _authorRepository.GetByIdAsync(id, cancellationToken);
        if (author == null)
            return null;

        author.FirstName = request.FirstName;
        author.LastName = request.LastName;
        author.Biography = request.Biography;
        author.DateOfBirth = request.DateOfBirth;
        author.DateOfDeath = request.DateOfDeath;
        author.Nationality = request.Nationality;
        author.UpdatedAt = DateTime.UtcNow;

        await _authorRepository.UpdateAsync(author, cancellationToken);
        return await GetAuthorByIdAsync(id, cancellationToken);
    }

    public async Task<bool> DeleteAuthorAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var author = await _authorRepository.GetByIdAsync(id, cancellationToken);
        if (author == null)
            return false;

        author.IsDeleted = true;
        author.UpdatedAt = DateTime.UtcNow;
        await _authorRepository.UpdateAsync(author, cancellationToken);
        return true;
    }

    // Book methods
    public async Task<PaginatedResult<BookDto>> GetBooksAsync(PaginationRequest request, Guid? categoryId = null, Guid? publisherId = null, string? search = null, CancellationToken cancellationToken = default)
    {
        var (books, totalCount) = await _bookRepository.GetPagedAsync(request, categoryId, publisherId, search, cancellationToken);

        var bookDtos = new List<BookDto>();
        foreach (var book in books)
        {
            var availableCopies = await _bookRepository.GetAvailableCopiesCountAsync(book.Id, cancellationToken);
            bookDtos.Add(new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                ISBN = book.ISBN,
                Subtitle = book.Subtitle,
                Edition = book.Edition,
                PublicationYear = book.PublicationYear,
                Pages = book.Pages,
                Language = book.Language,
                Description = book.Description,
                CoverImageUrl = book.CoverImageUrl,
                CategoryId = book.CategoryId,
                CategoryName = book.Category?.Name,
                PublisherId = book.PublisherId,
                PublisherName = book.Publisher?.Name,
                Authors = book.BookAuthors?.Select(ba => new AuthorDto
                {
                    Id = ba.Author.Id,
                    FirstName = ba.Author.FirstName,
                    LastName = ba.Author.LastName,
                    Biography = ba.Author.Biography,
                    DateOfBirth = ba.Author.DateOfBirth,
                    DateOfDeath = ba.Author.DateOfDeath,
                    Nationality = ba.Author.Nationality,
                    CreatedAt = ba.Author.CreatedAt
                }).ToList() ?? new List<AuthorDto>(),
                TotalCopies = book.BookCopies?.Count ?? 0,
                AvailableCopies = availableCopies,
                CreatedAt = book.CreatedAt
            });
        }

        return new PaginatedResult<BookDto>
        {
            Items = bookDtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<BookDto?> GetBookByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var book = await _bookRepository.GetByIdAsync(id, cancellationToken);
        if (book == null)
            return null;

        var availableCopies = await _bookRepository.GetAvailableCopiesCountAsync(book.Id, cancellationToken);

        return new BookDto
        {
            Id = book.Id,
            Title = book.Title,
            ISBN = book.ISBN,
            Subtitle = book.Subtitle,
            Edition = book.Edition,
            PublicationYear = book.PublicationYear,
            Pages = book.Pages,
            Language = book.Language,
            Description = book.Description,
            CoverImageUrl = book.CoverImageUrl,
            CategoryId = book.CategoryId,
            CategoryName = book.Category?.Name,
            PublisherId = book.PublisherId,
            PublisherName = book.Publisher?.Name,
            Authors = book.BookAuthors?.Select(ba => new AuthorDto
            {
                Id = ba.Author.Id,
                FirstName = ba.Author.FirstName,
                LastName = ba.Author.LastName,
                Biography = ba.Author.Biography,
                DateOfBirth = ba.Author.DateOfBirth,
                DateOfDeath = ba.Author.DateOfDeath,
                Nationality = ba.Author.Nationality,
                CreatedAt = ba.Author.CreatedAt
            }).ToList() ?? new List<AuthorDto>(),
            TotalCopies = book.BookCopies?.Count ?? 0,
            AvailableCopies = availableCopies,
            CreatedAt = book.CreatedAt
        };
    }

    public async Task<BookDto?> GetBookByISBNAsync(string isbn, CancellationToken cancellationToken = default)
    {
        var book = await _bookRepository.GetByISBNAsync(isbn, cancellationToken);
        if (book == null)
            return null;

        var availableCopies = await _bookRepository.GetAvailableCopiesCountAsync(book.Id, cancellationToken);

        return new BookDto
        {
            Id = book.Id,
            Title = book.Title,
            ISBN = book.ISBN,
            Subtitle = book.Subtitle,
            Edition = book.Edition,
            PublicationYear = book.PublicationYear,
            Pages = book.Pages,
            Language = book.Language,
            Description = book.Description,
            CoverImageUrl = book.CoverImageUrl,
            CategoryId = book.CategoryId,
            CategoryName = book.Category?.Name,
            PublisherId = book.PublisherId,
            PublisherName = book.Publisher?.Name,
            Authors = book.BookAuthors?.Select(ba => new AuthorDto
            {
                Id = ba.Author.Id,
                FirstName = ba.Author.FirstName,
                LastName = ba.Author.LastName,
                Biography = ba.Author.Biography,
                DateOfBirth = ba.Author.DateOfBirth,
                DateOfDeath = ba.Author.DateOfDeath,
                Nationality = ba.Author.Nationality,
                CreatedAt = ba.Author.CreatedAt
            }).ToList() ?? new List<AuthorDto>(),
            TotalCopies = book.BookCopies?.Count ?? 0,
            AvailableCopies = availableCopies,
            CreatedAt = book.CreatedAt
        };
    }

    public async Task<BookDto> CreateBookAsync(CreateBookRequest request, CancellationToken cancellationToken = default)
    {
        var isUnique = await _bookRepository.IsISBNUniqueAsync(request.ISBN, null, cancellationToken);
        if (!isUnique)
            throw new InvalidOperationException("ISBN already exists");

        var book = new Book
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            ISBN = request.ISBN,
            Subtitle = request.Subtitle,
            Edition = request.Edition,
            PublicationYear = request.PublicationYear,
            Pages = request.Pages,
            Language = request.Language,
            Description = request.Description,
            CoverImageUrl = request.CoverImageUrl,
            CategoryId = request.CategoryId,
            PublisherId = request.PublisherId,
            CreatedAt = DateTime.UtcNow
        };

        if (request.AuthorIds?.Any() == true)
        {
            foreach (var authorId in request.AuthorIds)
            {
                book.BookAuthors.Add(new BookAuthor { BookId = book.Id, AuthorId = authorId });
            }
        }

        await _bookRepository.AddAsync(book, cancellationToken);
        return await GetBookByIdAsync(book.Id, cancellationToken) ?? throw new InvalidOperationException("Failed to create book");
    }

    public async Task<BookDto?> UpdateBookAsync(Guid id, UpdateBookRequest request, CancellationToken cancellationToken = default)
    {
        var book = await _bookRepository.GetByIdAsync(id, cancellationToken);
        if (book == null)
            return null;

        if (!string.IsNullOrEmpty(request.ISBN) && request.ISBN != book.ISBN)
        {
            var isUnique = await _bookRepository.IsISBNUniqueAsync(request.ISBN, id, cancellationToken);
            if (!isUnique)
                throw new InvalidOperationException("ISBN already exists");
            book.ISBN = request.ISBN;
        }

        book.Title = request.Title;
        book.Subtitle = request.Subtitle;
        book.Edition = request.Edition;
        book.PublicationYear = request.PublicationYear;
        book.Pages = request.Pages;
        book.Language = request.Language;
        book.Description = request.Description;
        book.CoverImageUrl = request.CoverImageUrl;
        book.CategoryId = request.CategoryId ?? book.CategoryId;
        book.PublisherId = request.PublisherId ?? book.PublisherId;
        book.UpdatedAt = DateTime.UtcNow;

        if (request.AuthorIds != null)
        {
            book.BookAuthors.Clear();
            foreach (var authorId in request.AuthorIds)
            {
                book.BookAuthors.Add(new BookAuthor { BookId = book.Id, AuthorId = authorId });
            }
        }

        await _bookRepository.UpdateAsync(book, cancellationToken);
        return await GetBookByIdAsync(id, cancellationToken);
    }

    public async Task<bool> DeleteBookAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var book = await _bookRepository.GetByIdAsync(id, cancellationToken);
        if (book == null)
            return false;

        book.IsDeleted = true;
        book.UpdatedAt = DateTime.UtcNow;
        await _bookRepository.UpdateAsync(book, cancellationToken);
        return true;
    }

    // BookCopy methods
    public async Task<PaginatedResult<BookCopyDto>> GetBookCopiesAsync(PaginationRequest request, Guid? bookId = null, CopyStatus? status = null, CancellationToken cancellationToken = default)
    {
        var result = await _bookCopyRepository.GetPagedAsync(request, bookId, status, cancellationToken);

        return new PaginatedResult<BookCopyDto>
        {
            Items = result.Items.Select(bc => new BookCopyDto
            {
                Id = bc.Id,
                BookId = bc.BookId,
                BookTitle = bc.Book?.Title ?? string.Empty,
                Barcode = bc.Barcode,
                Status = bc.Status,
                Location = bc.Location,
                Condition = bc.Condition,
                AcquisitionDate = bc.AcquisitionDate,
                Cost = bc.Cost,
                CreatedAt = bc.CreatedAt
            }),
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };
    }

    public async Task<IEnumerable<BookSummaryDto>> SearchBooksAsync(string query, int maxResults = 10, CancellationToken cancellationToken = default)
    {
        var books = await _bookRepository.SearchAsync(query, maxResults, cancellationToken);

        return books.Select(b => new BookSummaryDto
        {
            Id = b.Id,
            Title = b.Title,
            ISBN = b.ISBN,
            CoverImageUrl = b.CoverImageUrl,
            CategoryName = b.Category?.Name ?? string.Empty,
            AuthorNames = b.BookAuthors.Select(ba => ba.Author?.FullName ?? string.Empty).ToList(),
            AvailableCopies = b.BookCopies.Count(bc => bc.Status == CopyStatus.Available)
        });
    }

    public async Task<BookCopyDto?> GetBookCopyByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var bookCopy = await _bookCopyRepository.GetByIdAsync(id, cancellationToken);
        if (bookCopy == null)
            return null;

        return new BookCopyDto
        {
            Id = bookCopy.Id,
            BookId = bookCopy.BookId,
            BookTitle = bookCopy.Book?.Title ?? string.Empty,
            Barcode = bookCopy.Barcode,
            Status = bookCopy.Status,
            Location = bookCopy.Location,
            Condition = bookCopy.Condition,
            AcquisitionDate = bookCopy.AcquisitionDate,
            Cost = bookCopy.Cost,
            CreatedAt = bookCopy.CreatedAt
        };
    }

    public async Task<BookCopyDto?> GetBookCopyByBarcodeAsync(string barcode, CancellationToken cancellationToken = default)
    {
        var bookCopy = await _bookCopyRepository.GetByBarcodeAsync(barcode, cancellationToken);
        if (bookCopy == null)
            return null;

        return new BookCopyDto
        {
            Id = bookCopy.Id,
            BookId = bookCopy.BookId,
            BookTitle = bookCopy.Book?.Title ?? string.Empty,
            Barcode = bookCopy.Barcode,
            Status = bookCopy.Status,
            Location = bookCopy.Location,
            Condition = bookCopy.Condition,
            AcquisitionDate = bookCopy.AcquisitionDate,
            Cost = bookCopy.Cost,
            CreatedAt = bookCopy.CreatedAt
        };
    }

    public async Task<BookCopyDto> CreateBookCopyAsync(CreateBookCopyRequest request, CancellationToken cancellationToken = default)
    {
        var isUnique = await _bookCopyRepository.IsBarcodeUniqueAsync(request.Barcode, null, cancellationToken);
        if (!isUnique)
            throw new InvalidOperationException("Barcode already exists");

        var bookCopy = new BookCopy
        {
            Id = Guid.NewGuid(),
            BookId = request.BookId,
            Barcode = request.Barcode,
            Status = CopyStatus.Available,
            Location = request.Location,
            Condition = request.Condition,
            AcquisitionDate = request.AcquisitionDate ?? DateTime.UtcNow,
            Cost = request.Cost ?? 0m,
            CreatedAt = DateTime.UtcNow
        };

        await _bookCopyRepository.AddAsync(bookCopy, cancellationToken);
        return await GetBookCopyByIdAsync(bookCopy.Id, cancellationToken) ?? throw new InvalidOperationException("Failed to create book copy");
    }

    public async Task<BookCopyDto?> UpdateBookCopyAsync(Guid id, UpdateBookCopyRequest request, CancellationToken cancellationToken = default)
    {
        var bookCopy = await _bookCopyRepository.GetByIdAsync(id, cancellationToken);
        if (bookCopy == null)
            return null;

        if (!string.IsNullOrEmpty(request.Barcode) && request.Barcode != bookCopy.Barcode)
        {
            var isUnique = await _bookCopyRepository.IsBarcodeUniqueAsync(request.Barcode, id, cancellationToken);
            if (!isUnique)
                throw new InvalidOperationException("Barcode already exists");
            bookCopy.Barcode = request.Barcode;
        }

        bookCopy.Status = request.Status ?? bookCopy.Status;
        bookCopy.Location = request.Location ?? bookCopy.Location;
        bookCopy.Condition = request.Condition ?? bookCopy.Condition;
        bookCopy.UpdatedAt = DateTime.UtcNow;

        await _bookCopyRepository.UpdateAsync(bookCopy, cancellationToken);
        return await GetBookCopyByIdAsync(id, cancellationToken);
    }

    public async Task<bool> DeleteBookCopyAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var bookCopy = await _bookCopyRepository.GetByIdAsync(id, cancellationToken);
        if (bookCopy == null)
            return false;

        bookCopy.IsDeleted = true;
        bookCopy.UpdatedAt = DateTime.UtcNow;
        await _bookCopyRepository.UpdateAsync(bookCopy, cancellationToken);
        return true;
    }

    public async Task<IEnumerable<BookCopyDto>> GetAvailableCopiesByBookAsync(Guid bookId, CancellationToken cancellationToken = default)
    {
        var copies = await _bookCopyRepository.GetAvailableByBookIdAsync(bookId, cancellationToken);
        return copies.Select(bc => new BookCopyDto
        {
            Id = bc.Id,
            BookId = bc.BookId,
            BookTitle = bc.Book?.Title ?? string.Empty,
            Barcode = bc.Barcode,
            Status = bc.Status,
            Location = bc.Location,
            Condition = bc.Condition,
            AcquisitionDate = bc.AcquisitionDate,
            Cost = bc.Cost,
            CreatedAt = bc.CreatedAt
        });
    }
}