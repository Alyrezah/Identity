using AutoMapper;
using Identity.Core.Application.DTOs.Account;
using Identity.Models.Account;

namespace Identity.Profiles
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<RegisterAccountDto,RegisterViewModel>().ReverseMap();
            CreateMap<LoginAccountDto,LoginViewModel>().ReverseMap();
        }
    }
}
