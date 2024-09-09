using Identity.Core.Application.DTOs.Account;

namespace Identity.Core.Application.Contracts.Identity
{
    public interface IIdentityService
    {
        Task<bool> IsEmailAlreadyExist(string email);
        Task<bool> IsUserNameAlreadyExist(string userName);
        Task<CommandResponse> RgisterAccount(RegisterAccountDto command);
        Task<CommandResponse> LoginAccount(LoginAccountDto command);
        Task Logout();
    }
}
