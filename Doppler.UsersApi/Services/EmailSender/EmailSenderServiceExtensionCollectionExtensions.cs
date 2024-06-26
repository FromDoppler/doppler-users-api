using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Doppler.UsersApi.Services.EmailSender
{
    public static class EmailSenderServiceCollectionExtensions
    {
        public static IServiceCollection AddRelayEmailSender(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .Configure<RelayEmailSenderSettings>(configuration.GetSection(nameof(RelayEmailSenderSettings)))
                .AddSingleton<IEmailSender, RelayEmailSender>()
                .AddOptions();
        }
    }
}
