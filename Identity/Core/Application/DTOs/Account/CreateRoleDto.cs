namespace Identity.Core.Application.DTOs.Account
{
    public class CreateRoleDto
    {
        public string Name { get; set; }
        public List<ActionAndControllerName> SitePath { get; set; }
    }
}
