using AutoMapper;
using Identity.Core.Application;
using Identity.Core.Application.Contracts.Identity;
using Identity.Core.Application.DTOs.Account;
using Identity.Core.Application.DTOs.Account.Validators;
using Microsoft.AspNetCore.Identity;

namespace Identity.Infrastructure.Idnetity.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IMapper _mapper;
        public IdentityService(UserManager<IdentityUser> userManager, IMapper mapper, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _mapper = mapper;
            _signInManager = signInManager;
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
            newUser.EmailConfirmed = true;
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
                    ErrorMessages = result.Errors.Select(x=>x.Description).ToList()
                };
            }

            return new CommandResponse()
            {
                IsSuccess = true,
                Message = CommandMessages.CommandSuccess
            };
        }
    }
}
