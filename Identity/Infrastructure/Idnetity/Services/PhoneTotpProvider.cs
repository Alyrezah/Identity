using Identity.Core.Application;
using Identity.Core.Application.Contracts.Identity;
using Identity.Infrastructure.Idnetity.Models;
using Microsoft.Extensions.Options;
using OtpNet;
using System.Text;

namespace Identity.Infrastructure.Idnetity.Services
{
    public class PhoneTotpProvider : IPhoneTotpProvider
    {
        private Totp _totp;
        private readonly PhoneTotpOptions _options;
        public PhoneTotpProvider(IOptions<PhoneTotpOptions> options)
        {
            _options = options?.Value ?? new PhoneTotpOptions();
        }

        public string GenerateTotpCode(string secretKey)
        {
            CreateTotp(secretKey);
            return _totp.ComputeTotp();
        }

        public PhoneTotpResponse VerifyTotpCode(string secretKey, string totpCode)
        {
            CreateTotp(secretKey);
            var isValid = _totp.VerifyTotp(totpCode, out _);

            if (isValid)
            {
                return new PhoneTotpResponse()
                {
                    IsSuccess = true,
                };
            }
            return new PhoneTotpResponse()
            {
                IsSuccess = false,
                ErrorMessage = "Invalid Code"
            };
        }

        private void CreateTotp(string secretKey)
        {
            _totp = new Totp(Encoding.UTF8.GetBytes(secretKey), _options.StepInSeconds);
        }
    }
}
