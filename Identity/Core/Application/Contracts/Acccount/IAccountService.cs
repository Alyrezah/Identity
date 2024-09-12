using Identity.Core.Application.DTOs.Account;

namespace Identity.Core.Application.Contracts.Acccount
{
    public interface IAccountService
    {
        Task<List<AccountDto>> GetList();
        Task<AccountDto> Getby(string id);
        Task<List<RoleDto>> GetRoles();
        Task<CommandResponse> AddRoleToUser(AddRoleToUserDto command);
        Task<AddRoleToUserDto> GetUserRoles(string userId);
    }
}
