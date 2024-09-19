namespace Identity.Core.Application.DTOs.Account
{
    public class ManageRoleClaimsDto
    {
        public string RoleId { get; set; }
        public List<ClaimsDto> _Claims { get; set; }

        public ManageRoleClaimsDto()
        {
            _Claims = new List<ClaimsDto>();
        }
    }
}
