namespace Identity.Infrastructure.Idnetity.Models
{
    public class PhoneTotpTempDataModel
    {
        public string SecretKey { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime ExpirtionTime { get; set; }
    }
}
