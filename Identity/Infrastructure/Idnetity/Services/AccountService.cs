using AutoMapper;
using Identity.Core.Application;
using Identity.Core.Application.Contracts.Acccount;
using Identity.Core.Application.DTOs.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
            var resetRoles = await _userManager.RemoveFromRolesAsync(user, removeRole);
            var selectedRole = command.Roles.Where(x => x.IsSelected).Select(x => x.RoleName);
            var reult = await _userManager.AddToRolesAsync(user, selectedRole);

            if (!reult.Succeeded)
            {
                return new CommandResponse()
                {
                    IsSuccess = false,
                    Message = CommandMessages.CommandFailer,
                    ErrorMessages = reult.Errors.Select(x => x.Description).ToList()
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
            var roles = await _roleManager.Roles.ToListAsync();

            if (user == null)
            {
                return null;
            }

            var rolesList = new List<AddRoleDto>();
            foreach (var role in roles)
            {
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    rolesList.Add(new AddRoleDto()
                    {
                        IsInRole = true,
                        RoleName = role.Name,
                    });
                }
                else
                {
                    rolesList.Add(new AddRoleDto()
                    {
                        IsInRole = false,
                        RoleName = role.Name,
                    });
                }
            }

            return new AddRoleToUserDto()
            {
                Roles = rolesList,
                UserId = user.Id
            };
        }
    }
}
