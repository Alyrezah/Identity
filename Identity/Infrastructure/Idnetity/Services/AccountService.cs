using AutoMapper;
using Identity.Core.Application;
using Identity.Core.Application.ClaimsStore;
using Identity.Core.Application.Contracts.Acccount;
using Identity.Core.Application.DTOs.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;

namespace Identity.Infrastructure.Idnetity.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        public AccountService(UserManager<IdentityUser> userManager, IMapper mapper,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _mapper = mapper;
            _roleManager = roleManager;
        }

        public async Task<CommandResponse> AddClaimsToUser(ManageClaimsDto command)
        {
            var user = await _userManager.FindByIdAsync(command.UserId);
            if (user == null)
            {
                return new CommandResponse()
                {
                    IsSuccess = false,
                    Message = CommandMessages.CommandFailer,
                    ErrorMessages = new List<string>() { "User not found" }
                };
            }

            var selectedClaims = command._Claims.Where(x => x.IsSelected)
                .Select(x => new Claim(x.ClaimType, true.ToString()))
                .ToList();

            var result = await _userManager.AddClaimsAsync(user, selectedClaims);

            if (!result.Succeeded)
            {
                return new CommandResponse()
                {
                    IsSuccess = false,
                    Message = CommandMessages.CommandFailer,
                    ErrorMessages = result.Errors.Select(x => x.Description).ToList()
                };
            }

            return new CommandResponse()
            {
                IsSuccess = true,
                Message = CommandMessages.CommandSuccess,
            };
        }

        public async Task<CommandResponse> RemoveClaimsFromUser(ManageClaimsDto command)
        {
            var user = await _userManager.FindByIdAsync(command.UserId);
            if (user == null)
            {
                return new CommandResponse()
                {
                    IsSuccess = false,
                    Message = CommandMessages.CommandFailer,
                    ErrorMessages = new List<string>() { "User not found" }
                };
            }

            var selectedClaims = command._Claims.Where(x => x.IsSelected)
                    .Select(x => new Claim(x.ClaimType, true.ToString()))
                    .ToList();

            var result = await _userManager.RemoveClaimsAsync(user, selectedClaims);

            if (!result.Succeeded)
            {
                return new CommandResponse()
                {
                    IsSuccess = false,
                    Message = CommandMessages.CommandFailer,
                    ErrorMessages = result.Errors.Select(x => x.Description).ToList()
                };
            }

            return new CommandResponse()
            {
                IsSuccess = true,
                Message = CommandMessages.CommandSuccess,
            };
        }

        public async Task<CommandResponse> AddRoleToUser(AddRoleToUserDto command)
        {
            var user = await _userManager.FindByIdAsync(command.UserId);
            if (user == null)
            {
                return new CommandResponse()
                {
                    IsSuccess = false,
                    Message = CommandMessages.CommandFailer,
                    ErrorMessages = new List<string>() { "User not found" }
                };
            }

            var removeRole = command.Roles.Where(x => x.IsInRole).Select(x => x.RoleName);
            await _userManager.RemoveFromRolesAsync(user, removeRole);
            var selectedRole = command.Roles.Where(x => x.IsSelected).Select(x => x.RoleName);
            var result = await _userManager.AddToRolesAsync(user, selectedRole);

            if (!result.Succeeded)
            {
                return new CommandResponse()
                {
                    IsSuccess = false,
                    Message = CommandMessages.CommandFailer,
                    ErrorMessages = result.Errors.Select(x => x.Description).ToList()
                };
            }

            return new CommandResponse()
            {
                IsSuccess = true,
                Message = CommandMessages.CommandSuccess,
            };
        }

        public async Task<AccountDto> Getby(string id)
        {
            var account = await _userManager.FindByIdAsync(id);
            return _mapper.Map<AccountDto>(account);
        }

        public async Task<ManageClaimsDto> GetClaimsForAddClaims(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return null;
            }

            var allClaims = ClaimsStore.WebsiteClaims;
            var userClaims = await _userManager.GetClaimsAsync(user);

            var validClaims = allClaims
                .Where(x => userClaims.All(c => c.Type != x.Type))
                .Select(x => new ClaimsDto()
                {
                    ClaimType = x.Type,
                }).ToList();

            return new ManageClaimsDto()
            {
                UserId = userId,
                _Claims = validClaims
            };
        }

        public async Task<ManageClaimsDto> GetClaimsForRemoveClaims(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return null;
            }

            var userClaims = await _userManager.GetClaimsAsync(user);

            var validClaims = userClaims.Select(x => new ClaimsDto()
                {
                    ClaimType = x.Type,
                }).ToList();

            return new ManageClaimsDto()
            {
                UserId = userId,
                _Claims = validClaims
            };
        }

        public async Task<List<AccountDto>> GetList()
        {
            var users = await _userManager.Users.ToListAsync();
            return _mapper.Map<List<AccountDto>>(users);
        }

        public async Task<List<RoleDto>> GetRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return _mapper.Map<List<RoleDto>>(roles);
        }

        public async Task<AddRoleToUserDto> GetUserRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return null;
            }

            var roles = await _roleManager.Roles.Select(x => x.Name).ToListAsync();
            var userRoles = await _userManager.GetRolesAsync(user);

            var rolesList = new List<AddRoleDto>();
            foreach (var role in roles)
            {
                if (userRoles.Contains(role))
                {
                    rolesList.Add(new AddRoleDto()
                    {
                        IsInRole = true,
                        RoleName = role,
                    });
                }
                else
                {
                    rolesList.Add(new AddRoleDto()
                    {
                        IsInRole = false,
                        RoleName = role,
                    });
                }
            }

            return new AddRoleToUserDto()
            {
                Roles = rolesList,
                UserId = user.Id
            };
        }

        public async Task<string> ReturnUserNameBy(string id)
        {
            return await _userManager.Users
                 .Where(x => x.Id == id)
                 .Select(x => x.UserName).FirstAsync();
        }
    }
}
