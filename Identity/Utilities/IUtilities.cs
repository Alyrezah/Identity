using Identity.Core.Application.DTOs.Account;

namespace IdentitySample.Repositories
{
    public interface IUtilities
    {
        List<ActionAndControllerName> ActionAndControllerNamesList();
        List<string> GetAllAreasNames();
        Task<string> DataBaseRoleValidationGuid();
    }
}
