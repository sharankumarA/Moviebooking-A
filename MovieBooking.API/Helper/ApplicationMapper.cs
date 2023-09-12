using AutoMapper;
using MovieBooking.API.Models;
using MovieBooking.API.Models.DTO;
using MovieBooking.API.Models.Entities;

namespace MovieBooking.API.Helper
{
    public class ApplicationMapper : Profile
    {
        public ApplicationMapper()
        {
            CreateMap<UserDto, User>().ReverseMap();
        }


    }
}
