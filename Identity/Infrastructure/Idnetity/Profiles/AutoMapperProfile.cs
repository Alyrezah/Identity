using AutoMapper;
using Identity.Core.Application.DTOs.Account;
using Identity.Core.Application.DTOs.SiteSetting;
using Identity.Infrastructure.Idnetity.Models;
using Microsoft.AspNetCore.Identity;

namespace Identity.Infrastructure.Idnetity.Profiles
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<IdentityUser,RegisterAccountDto>().ReverseMap();
            CreateMap<IdentityUser,AccountDto>().ReverseMap();
            CreateMap<IdentityRole,RoleDto>().ReverseMap();
            CreateMap<IdentityRole,CreateRoleDto>().ReverseMap();
            CreateMap<SiteSetting,SiteSettingDto>().ReverseMap();
        }
    }
}
