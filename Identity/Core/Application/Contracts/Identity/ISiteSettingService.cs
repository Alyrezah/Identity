using Identity.Core.Application.DTOs.SiteSetting;

namespace Identity.Core.Application.Contracts.Identity
{
    public interface ISiteSettingService
    {
        Task<List<SiteSettingDto>> GetList();
        Task<SiteSettingDto> GetRoleValidationGuid();
        Task Update(SiteSettingDto command);
        Task Create();
    }
}
