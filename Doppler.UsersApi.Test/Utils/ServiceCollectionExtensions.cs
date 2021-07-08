using Doppler.UsersApi.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Data.Common;

namespace Doppler.UsersApi.Test.Utils
{
    public static class ServiceCollectionExtensions
    {
        public static void SetupConnectionFactory(this IServiceCollection services, DbConnection dbConnection)
        {
            var mockDatabaseConnectionFactory = new Mock<IDatabaseConnectionFactory>();
            mockDatabaseConnectionFactory.Setup(a => a.GetConnection()).ReturnsAsync(dbConnection);
            services.AddSingleton(mockDatabaseConnectionFactory.Object);
        }

    }
}
