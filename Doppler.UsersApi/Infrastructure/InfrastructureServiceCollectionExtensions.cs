using Doppler.UsersApi.Infrastructure;
using Doppler.UsersApi.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class InfrastructureServiceCollectionExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IFeaturesRepository, FeaturesRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IDatabaseConnectionFactory, DatabaseConnectionFactory>();
            services.AddScoped<IIntegrationsRepository, IntegrationsRepository>();
            services.AddScoped<IAccountService, AccountService>();
            return services;
        }
    }
}
