using AutoMapper;
using BookWebAPI.Dtos.Identity;
using BookWebAPI.Models;

namespace BookWebAPI.Profiles
{
    public class ApplicationUserProfile : Profile
    {
        public ApplicationUserProfile()
        {
            CreateMap<ApplicationUser, AuthenticationResponseModel>()
                .ForMember(x => x.UserId, y => y.MapFrom(x => x.Id))
                .ReverseMap();
            CreateMap<ApplicationUser, RegisterRequestModel>().ReverseMap();
            CreateMap<ApplicationUser, LoginRequestModel>().ReverseMap();
        }
    }
}
