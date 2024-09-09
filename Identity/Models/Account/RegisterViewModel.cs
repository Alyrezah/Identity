using Microsoft.AspNetCore.Mvc;

namespace Identity.Models.Account
{
    public class RegisterViewModel
    {
        [Remote(action: "IsUserNameAlredyExist", controller: "Account")]
        public string UserName { get; set; }

        [Remote(action: "IsEmailAlredyExist", controller: "Account")]
        public string Email { get; set; }
        public string Password { get; set; }
        public string RePassword { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
