using Doppler.UsersApi.Infrastructure;

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
            services.AddScoped<IAccountPlansRepository, AccountPlansRepository>();
            return services;
        }
    }
}
