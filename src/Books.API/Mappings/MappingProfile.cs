using AutoMapper;
using Books.API.DTOs;
using Books.Core.Entities;

namespace Books.API.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Book, BookDto>();
        CreateMap<BookDto, Book>();
    }
}