using AutoMapper;
using CodeChallenge.Entities;
using CodeChallenge.Models.Request;

namespace CodeChallenge.Helper
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            CreateMap<ApplicationUser, UserRegistrationRequestModel>().ReverseMap();

        }
    }
}