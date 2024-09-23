using Identity.Core.Application.DTOs.Account;
using Identity.Infrastructure.Idnetity.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace Identity.Core.Application.Contracts.Identity
{
    public interface IIdentityService
    {
        Task<bool> IsEmailAlreadyExist(string email);
        Task<bool> IsUserNameAlreadyExist(string userName);
        Task<CommandResponse> RgisterAccount(RegisterAccountDto command);
        Task<CommandResponse> LoginAccount(LoginAccountDto command);
        Task<PhoneTotpTempDataModel> SenTotpCode(LoginWithPhoneNumberDto command);
        Task<CommandResponse> LoginWithPhoneNumber(VerifyTotpCodeDto command, byte[] secretKey,string phoneNumber);
        Task Logout();
        Task<CommandResponse> ConfirmEmail(string userName, string token);
        Task<List<AuthenticationScheme>> GetExternalLogins();
        AuthenticationProperties ConfigureExternalLoginProperties(string provider, string returnUrl);
        Task<ExternalLoginInfo> GetExternalLoginInfo();
        Task<CommandResponse> ExternalLogin(ExternalLoginInfo externalLoginInfo);
        Task<CommandResponse> RegisterUserWithExternalLogin(string email, RegisterWithExternalLoginDto command, ExternalLoginInfo externalLoginInfo);
        Task RefreshCookie(string userId);
        Task<CommandResponse> SendResetPasswordEmail(ForgotPasswordDto command);
        Task<CommandResponse> ResetPassword(ResetPasswordDto command);
    }
}
