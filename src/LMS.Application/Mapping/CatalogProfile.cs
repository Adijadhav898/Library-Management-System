using AutoMapper;
using LMS.Application.DTOs;
using LMS.Domain.Entities;

namespace LMS.Application.Mapping;

public class CatalogProfile : Profile
{
    public CatalogProfile()
    {
        CreateMap<Category, CategoryDto>()
            .ForMember(dest => dest.ParentCategoryName, opt => opt.MapFrom(src => src.ParentCategory != null ? src.ParentCategory.Name : string.Empty));

        CreateMap<CreateCategoryRequest, Category>();
        CreateMap<UpdateCategoryRequest, Category>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<Publisher, PublisherDto>();

        CreateMap<CreatePublisherRequest, Publisher>();
        CreateMap<UpdatePublisherRequest, Publisher>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<Author, AuthorDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FirstName + " " + src.LastName));

        CreateMap<CreateAuthorRequest, Author>();
        CreateMap<UpdateAuthorRequest, Author>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<Book, BookDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty))
            .ForMember(dest => dest.PublisherName, opt => opt.MapFrom(src => src.Publisher != null ? src.Publisher.Name : string.Empty))
            .ForMember(dest => dest.Authors, opt => opt.MapFrom(src => src.BookAuthors.Select(ba => ba.Author)))
            .ForMember(dest => dest.AvailableCopies, opt => opt.MapFrom(src => src.Copies.Count(c => c.Status == Domain.Enums.CopyStatus.Available)))
            .ForMember(dest => dest.TotalCopies, opt => opt.MapFrom(src => src.Copies.Count));

        CreateMap<CreateBookRequest, Book>()
            .ForMember(dest => dest.BookAuthors, opt => opt.Ignore());

        CreateMap<UpdateBookRequest, Book>()
            .ForMember(dest => dest.BookAuthors, opt => opt.Ignore())
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<BookCopy, BookCopyDto>()
            .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book != null ? src.Book.Title : string.Empty))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));

        CreateMap<CreateBookCopyRequest, BookCopy>();
        CreateMap<UpdateBookCopyRequest, BookCopy>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}