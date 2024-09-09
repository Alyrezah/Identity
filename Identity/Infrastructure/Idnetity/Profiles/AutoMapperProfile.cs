using AutoMapper;
using Identity.Core.Application.DTOs.Account;
using Microsoft.AspNetCore.Identity;

namespace Identity.Infrastructure.Idnetity.Profiles
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<IdentityUser,RegisterAccountDto>().ReverseMap();
        }
    }
}
