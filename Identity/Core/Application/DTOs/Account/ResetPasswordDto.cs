namespace Identity.Core.Application.DTOs.Account
{
    public class ResetPasswordDto
    {
        public string NewPassword { get; set; }
        public string ReNewPassword { get; set; }

        public string UserName { get; set; } 
        public string Token { get; set; }
    }
}
