namespace Identity.Core.Application.Contracts.Identity
{
    public interface IPhoneTotpProvider
    {
        string GenerateTotpCode(byte[] secretKey);
        PhoneTotpResponse VerifyTotpCode(byte[] secretKey, string totpCode);
    }
}
