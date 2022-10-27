using AutoMapper;
using BookWebAPI.Dtos.Books;
using BookWebAPI.Models;

namespace BookWebAPI.Profiles
{
    public class BookProfile : Profile
    {
        public BookProfile()
        {
            //oshte logika
            CreateMap<Book, InputBookDto>().ReverseMap();
            CreateMap<Book, OutputBookDto>()
                .ForMember(x => x.AuthorFullName,y => y.MapFrom(b => b.Author.FirstName + " "+ b.Author.LastName))
                .ForMember(x => x.Genre,y => y.MapFrom(b => b.Genre.Name))
                .ReverseMap();
               
        }
    }
}
