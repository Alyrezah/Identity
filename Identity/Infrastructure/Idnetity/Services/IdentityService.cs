using AutoMapper;
using Identity.Core.Application;
using Identity.Core.Application.Contracts.Identity;
using Identity.Core.Application.Contracts.Infrastructure;
using Identity.Core.Application.DTOs.Account;
using Identity.Core.Application.DTOs.Account.Validators;
using Identity.Infrastructure.Idnetity.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Identity.Infrastructure.Idnetity.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IMapper _mapper;
        private readonly IMessageSender _messageSender;
        private readonly IPhoneTotpProvider _phoneTotpProvider;
        private readonly PhoneTotpOptions _options;
        public IdentityService(UserManager<IdentityUser> userManager, IMapper mapper,
            SignInManager<IdentityUser> signInManager, IMessageSender messageSender, IPhoneTotpProvider phoneTotpProvider
            , IOptions<PhoneTotpOptions> options)
        {
            _userManager = userManager;
            _mapper = mapper;
            _signInManager = signInManager;
            _messageSender = messageSender;
            _phoneTotpProvider = phoneTotpProvider;
            _options = options?.Value ?? new PhoneTotpOptions();
        }

        public async Task<CommandResponse> ConfirmEmail(string userName, string token)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return new CommandResponse()
                {
                    IsSuccess = false,
                    Message = CommandMessages.CommandFailer,
                    ErrorMessages = new List<string>() { "User Not Found ..." }
                };
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);

            return new CommandResponse()
            {
                IsSuccess = true,
                Message = CommandMessages.CommandSuccess,
            };
        }

        public AuthenticationProperties ConfigureExternalLoginProperties(string provider, string returnUrl)
        {
            return _signInManager.ConfigureExternalAuthenticationProperties(provider, returnUrl);
        }

        public async Task<List<AuthenticationScheme>> GetExternalLogins()
        {
            return (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<bool> IsEmailAlreadyExist(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            return user != null;
        }

        public async Task<bool> IsUserNameAlreadyExist(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);

            return user != null;
        }

        public async Task<CommandResponse> LoginAccount(LoginAccountDto command)
        {
            var validator = new LoginValidator();
            var validatorResult = await validator.ValidateAsync(command);
            if (!validatorResult.IsValid)
            {
                return new CommandResponse()
                {
                    IsSuccess = false,
                    Message = CommandMessages.CommandFailer,
                    ErrorMessages = validatorResult.Errors.Select(x => x.ErrorMessage).ToList()
                };
            }

            var user = await _userManager.FindByEmailAsync(command.Email);

            if (user == null)
            {
                return new CommandResponse()
                {
                    IsSuccess = false,
                    Message = CommandMessages.CommandFailer,
                    ErrorMessages = new List<string>() { "The information entered is incorrect." }
                };
            }

#pragma warning disable CS8604 // Possible null reference argument.
            var result = await _signInManager.PasswordSignInAsync(user, command.Password, command.RememberMe, true);
#pragma warning restore CS8604 // Possible null reference argument.

            if (result.Succeeded)
            {
                return new CommandResponse()
                {
                    IsSuccess = true,
                    Message = CommandMessages.CommandSuccess
                };
            }

            if (result.IsLockedOut)
            {
                return new CommandResponse()
                {
                    IsSuccess = false,
                    Message = CommandMessages.CommandFailer,
                    ErrorMessages = new List<string>() { "Your account has been locked for 5 minutes due to repeated requests." }
                };
            }

            if (result.IsNotAllowed)
            {
                return new CommandResponse()
                {
                    IsSuccess = false,
                    Message = CommandMessages.CommandFailer,
                    ErrorMessages = new List<string>() { "You are not allowed to enter" }
                };
            }

            return new CommandResponse()
            {
                IsSuccess = false,
                Message = CommandMessages.CommandFailer,
                ErrorMessages = new List<string>() { "The information entered is incorrect." }
            };
        }

        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<CommandResponse> RgisterAccount(RegisterAccountDto command)
        {
            var validator = new RegisterValidator();
            var validatorResult = await validator.ValidateAsync(command);
            if (!validatorResult.IsValid)
            {
                return new CommandResponse()
                {
                    IsSuccess = false,
                    Message = CommandMessages.CommandFailer,
                    ErrorMessages = validatorResult.Errors.Select(x => x.ErrorMessage).ToList()
                };
            }


            if (await _userManager.FindByEmailAsync(command.Email) != null)
            {
                return new CommandResponse()
                {
                    IsSuccess = false,
                    Message = CommandMessages.CommandFailer,
                    ErrorMessages = new List<string>() { "The email entered has already been registered" }
                };
            }

            if (await _userManager.FindByNameAsync(command.UserName) != null)
            {
                return new CommandResponse()
                {
                    IsSuccess = false,
                    Message = CommandMessages.CommandFailer,
                    ErrorMessages = new List<string>() { "The username entered has already been registered" }
                };
            }

            var newUser = _mapper.Map<IdentityUser>(command);
            //newUser.EmailConfirmed = true;
            if (command.PhoneNumber != null)
            {
                newUser.PhoneNumberConfirmed = true;
            }
            var result = await _userManager.CreateAsync(newUser, command.Password);

            if (!result.Succeeded)
            {
                return new CommandResponse()
                {
                    IsSuccess = false,
                    Message = CommandMessages.CommandFailer,
                    ErrorMessages = result.Errors.Select(x => x.Description).ToList()
                };
            }

            #region Send Email

            var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);

            var emailLink = $"https://localhost:7040/Account/ConfirmEmail?username={newUser.UserName}&token={emailConfirmationToken}";

            _messageSender.SendEmail(command.Email, "ConfirmationEmail", emailLink);

            #endregion


            return new CommandResponse()
            {
                IsSuccess = true,
                Message = CommandMessages.CommandSuccess
            };
        }

        public async Task<ExternalLoginInfo> GetExternalLoginInfo()
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await _signInManager.GetExternalLoginInfoAsync();
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task<CommandResponse> ExternalLogin(ExternalLoginInfo externalLoginInfo)
        {
            var result = await _signInManager.ExternalLoginSignInAsync(externalLoginInfo.LoginProvider,
                externalLoginInfo.ProviderKey, true, true);

            if (result.Succeeded)
            {
                return new CommandResponse()
                {
                    IsSuccess = true,
                    Message = CommandMessages.CommandSuccess
                };
            }

            if (result.IsLockedOut)
            {
                return new CommandResponse()
                {
                    IsSuccess = false,
                    Message = CommandMessages.CommandFailer,
                    ErrorMessages = new List<string>() { "Your account has been locked for 5 minutes due to repeated requests." }
                };
            }

            if (result.IsNotAllowed)
            {
                return new CommandResponse()
                {
                    IsSuccess = false,
                    Message = CommandMessages.CommandFailer,
                    ErrorMessages = new List<string>() { "You are not allowed to enter" }
                };
            }

            return new CommandResponse()
            {
                IsSuccess = false,
                Message = CommandMessages.CommandFailer,
                ErrorMessages = new List<string>() { "The information entered is incorrect." }
            };
        }

        public async Task<CommandResponse> RegisterUserWithExternalLogin(string email,
            RegisterWithExternalLoginDto command, ExternalLoginInfo externalLoginInfo)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new IdentityUser()
                {
                    Email = email,
                    UserName = command.UserName,
                    PhoneNumber = command.PhoneNumber,
                    EmailConfirmed = true,
                };
                if (command.PhoneNumber != null)
                {
                    user.PhoneNumberConfirmed = true;
                }
                await _userManager.CreateAsync(user);
            }
            await _userManager.AddLoginAsync(user, externalLoginInfo);
            await _signInManager.SignInAsync(user, true);

            return new CommandResponse()
            {
                IsSuccess = true,
                Message = CommandMessages.CommandSuccess,
            };
        }

        public async Task RefreshCookie(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            await _signInManager.RefreshSignInAsync(user);
        }

        public async Task<CommandResponse> SendResetPasswordEmail(ForgotPasswordDto command)
        {
            var validator = new ForgotPasswordValidator();
            var validatorResult = await validator.ValidateAsync(command);

            if (!validatorResult.IsValid)
            {
                return new CommandResponse()
                {
                    ErrorMessages = validatorResult.Errors.Select(x => x.ErrorMessage).ToList(),
                    IsSuccess = false,
                    Message = CommandMessages.CommandFailer
                };
            }

            var user = await _userManager.FindByEmailAsync(command.Email);
            if (user != null)
            {
                var resetPasswordToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                var emailLink = $"https://localhost:7040/Account/ResetPassword?username={user.UserName}&token={resetPasswordToken}";

                //_messageSender.SendEmail(command.Email, "Reset Password Link", emailLink);
            }

            return new CommandResponse()
            {
                IsSuccess = true,
                Message = CommandMessages.CommandSuccess,
            };
        }

        public async Task<CommandResponse> ResetPassword(ResetPasswordDto command)
        {
            var validator = new ResetPasswordValidator();
            var validatorResult = await validator.ValidateAsync(command);

            if (!validatorResult.IsValid)
            {
                return new CommandResponse()
                {
                    ErrorMessages = validatorResult.Errors.Select(x => x.ErrorMessage).ToList(),
                    IsSuccess = false,
                    Message = CommandMessages.CommandFailer
                };
            }

            var user = await _userManager.FindByNameAsync(command.UserName);
            if (user == null)
            {
                return new CommandResponse()
                {
                    ErrorMessages = new List<string>() { "User not found" },
                    IsSuccess = false,
                    Message = CommandMessages.CommandFailer
                };
            }

            var result = await _userManager.ResetPasswordAsync(user, command.Token, command.NewPassword);
            if (!result.Succeeded)
            {
                return new CommandResponse()
                {
                    ErrorMessages = result.Errors.Select(x => x.Description).ToList(),
                    IsSuccess = false,
                    Message = CommandMessages.CommandFailer
                };
            }

            return new CommandResponse()
            {
                IsSuccess = true,
                Message = CommandMessages.CommandSuccess
            };
        }

        public async Task<PhoneTotpTempDataModel> SenTotpCode(LoginWithPhoneNumberDto command)
        {
            var secretKey = Guid.NewGuid().ToString();
            var totpCode = _phoneTotpProvider.GenerateTotpCode(secretKey);

            var isUserExist = await _userManager.Users.AnyAsync(x => x.PhoneNumber == command.PhoneNumber);
            if (isUserExist)
            {
                //TODO send code
            }

            return new PhoneTotpTempDataModel()
            {
                ExpirtionTime = DateTime.Now.AddSeconds(_options.StepInSeconds),
                PhoneNumber = command.PhoneNumber,
                SecretKey = secretKey
            };
        }

        public async Task<CommandResponse> LoginWithPhoneNumber(VerifyTotpCodeDto command, string secretKey, string phoneNumber)
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);

            if (user == null)
            {
                return new CommandResponse()
                {
                    IsSuccess = false,
                    Message = CommandMessages.CommandFailer,
                    ErrorMessages = new List<string>() { "An error occurred during the execution of the operation" }
                };
            }

            var result = _phoneTotpProvider.VerifyTotpCode(secretKey, command.TotpCode);

            if (!result.IsSuccess)
            {
                if (!await _userManager.IsLockedOutAsync(user))
                {
                    await _userManager.AccessFailedAsync(user);
                }

                return new CommandResponse()
                {
                    IsSuccess = false,
                    Message = CommandMessages.CommandFailer,
                    ErrorMessages = new List<string>() { result.ErrorMessage }
                };
            }

            if (await _userManager.IsLockedOutAsync(user))
            {
                return new CommandResponse()
                {
                    IsSuccess = false,
                    Message = CommandMessages.CommandFailer,
                    ErrorMessages = new List<string>() { "Your account is locked" }
                };
            }
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.MobilePhone,phoneNumber)
            };
            await _userManager.ResetAccessFailedCountAsync(user);
            await _signInManager.SignInWithClaimsAsync(user, true, claims);

            return new CommandResponse()
            {
                IsSuccess = true,
                Message = CommandMessages.CommandSuccess,
            };
        }
    }
}
