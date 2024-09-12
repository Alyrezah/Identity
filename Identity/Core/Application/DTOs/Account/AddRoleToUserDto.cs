namespace Identity.Core.Application.DTOs.Account
{
    public class AddRoleToUserDto
    {
        public AddRoleToUserDto()
        {
            Roles = new List<AddRoleDto>();
        }
        public List<AddRoleDto> Roles { get; set; }
        public string UserId { get; set; }
    }
}
