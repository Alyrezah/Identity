namespace Identity.Core.Application.DTOs.Account
{
    public class AddRoleDto
    {
        public string RoleName { get; set; }
        public bool IsSelected { get; set; }
        public bool IsInRole { get; set; }
    }
}