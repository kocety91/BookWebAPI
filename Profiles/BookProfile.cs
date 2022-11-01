using AutoMapper;
using BookWebAPI.Dtos.Books;
using BookWebAPI.Models;

namespace BookWebAPI.Profiles
{
    public class BookProfile : Profile
    {
        public BookProfile()
        {
            CreateMap<Book, InputBookDto>().ReverseMap();
            CreateMap<Book, OutputBookDto>()
                .ForMember(x => x.AuthorFullName,y => y.MapFrom(b => b.Author.FirstName + " "+ b.Author.LastName))
                .ForMember(x => x.Genre,y => y.MapFrom(b => b.Genre.Name))
                .ForMember(x => x.AddedByUser, y => y.MapFrom(b => b.ApplicationUser.UserName))
                .ReverseMap();
               
        }
    }
}
