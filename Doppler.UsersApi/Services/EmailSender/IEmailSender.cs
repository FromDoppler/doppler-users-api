using System.Threading;
using System.Threading.Tasks;

namespace Doppler.UsersApi.Services.EmailSender
{
    public interface IEmailSender
    {
        Task SendEmailAsync(EmailData emailData, CancellationToken cancellationToken = default);
        Task SendTemplatedEmailAsync(EmailData emailData, CancellationToken cancellationToken = default);
    }
}
