namespace Identity.Core.Application.Contracts.Identity
{
    public interface IPhoneTotpProvider
    {
        string GenerateTotpCode(string secretKey);
        PhoneTotpResponse VerifyTotpCode(string secretKey, string totpCode);
    }
}
