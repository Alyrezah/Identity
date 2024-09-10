namespace Identity.Core.Application.Contracts.Infrastructure
{
    public interface IMessageSender
    {
        public Task SendEmailAsync(string toEmail, string subject, string message, bool isMessageHtml = false);
        void SendEmail(string To, string Subject, string Body);
    }
}
