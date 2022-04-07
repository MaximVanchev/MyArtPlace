using MyArtPlace.Core.Models.Mail;

namespace MyArtPlace.Core.Contracts
{
    public interface IMailService
    {
        Task SendEmailTestAsync(MailRequest mailRequest);

        Task SendEmailWithMessageAsync(string emailId, string username, string message);
    }
}
