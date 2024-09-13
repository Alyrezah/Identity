namespace Identity.Core.Application.DTOs.Account
{
    public class ManageClaimsDto
    {
        public string UserId { get; set; }
        public List<ClaimsDto> _Claims { get; set; }

        public ManageClaimsDto()
        {
            _Claims = new List<ClaimsDto>();
        }
    }

    public class ClaimsDto
    {
        public string ClaimType { get; set; }
        public bool IsSelected { get; set; }
    }
}
