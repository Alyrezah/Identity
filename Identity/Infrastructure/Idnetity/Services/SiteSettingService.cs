using Identity.Core.Application.Contracts.Identity;
using Identity.Core.Application.DTOs.SiteSetting;
using Identity.Infrastructure.Idnetity.Models;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Idnetity.Services
{
    public class SiteSettingService : ISiteSettingService
    {
        private readonly IdentityContext _context;
        public SiteSettingService(IdentityContext context)
        {
            _context = context;
        }

        public async Task Create()
        {
            var siteSetting = new SiteSetting()
            {
                Key = "RoleValidationGuid",
                Value = Guid.NewGuid().ToString(),
                LastModifiedDate = DateTime.Now,
            };
            await _context.AddAsync(siteSetting);
            await _context.SaveChangesAsync();
        }

        public async Task<List<SiteSettingDto>> GetList()
        {
            return await _context.SiteSetting
                .Select(x => new SiteSettingDto()
                {
                    Key = x.Key,
                    Value = x.Value,
                    LastModifiedDate = x.LastModifiedDate,
                }).ToListAsync();
        }

        public Task<SiteSettingDto> GetRoleValidationGuid()
        {
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
            return _context.SiteSetting.Where(x => x.Key == "RoleValidationGuid")
                .Select(x => new SiteSettingDto()
                {
                    Key = x.Key,
                    Value = x.Value,
                    LastModifiedDate = x.LastModifiedDate,
                }).FirstOrDefaultAsync();
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
        }

        public async Task Update(SiteSettingDto command)
        {
            var roleValidationGuid = await _context.SiteSetting.FirstOrDefaultAsync(x => x.Key == "RoleValidationGuid");

            roleValidationGuid.Value = Guid.NewGuid().ToString();
            roleValidationGuid.LastModifiedDate = DateTime.Now;
            _context.Update(roleValidationGuid);
            await _context.SaveChangesAsync();
        }
    }
}
