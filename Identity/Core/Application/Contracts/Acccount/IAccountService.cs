using Identity.Core.Application.DTOs.Account;

namespace Identity.Core.Application.Contracts.Acccount
{
    public interface IAccountService
    {
        Task<List<AccountDto>> GetList();
        Task<AccountDto> Getby(string id);
        Task<List<RoleDto>> GetRoles();
        Task<CommandResponse> CreateRole(CreateRoleDto command);
        Task<CommandResponse> AddRoleToUser(AddRoleToUserDto command);
        Task<AddRoleToUserDto> GetUserRoles(string userId);
        Task<ManageClaimsDto> GetClaimsForAddClaims(string userId);
        Task<ManageRoleClaimsDto> GetRoleClaimsForAddRoleClaims(string roleId, List<ActionAndControllerName> paths);
        Task<ManageClaimsDto> GetClaimsForRemoveClaims(string userId);
        Task<string> ReturnUserNameBy(string id);
        Task<CommandResponse> AddClaimsToUser(ManageClaimsDto command);
        Task<CommandResponse> RemoveClaimsFromUser(ManageClaimsDto command);
        Task<CommandResponse> UpdateSecurityStamp(string userId);
        Task<CommandResponse> AddCliamsToRole(ManageRoleClaimsDto command);
    }
}
