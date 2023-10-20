using Doppler.UsersApi.Infrastructure;
using Doppler.UsersApi.Model;
using Doppler.UsersApi.Services;
using Doppler.UsersApi.Test.Utils;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Doppler.UsersApi.Test
{
    public class GetPlanInformationTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        const string TOKEN_ACCOUNT_123_TEST1_AT_TEST_DOT_COM_EXPIRE_20330518 = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOjEyMywidW5pcXVlX25hbWUiOiJ0ZXN0MUB0ZXN0LmNvbSIsInJvbGUiOiJVU0VSIiwiZXhwIjoyMDAwMDAwMDAwfQ.E3RHjKx9p0a-64RN2YPtlEMysGM45QBO9eATLBhtP4tUQNZnkraUr56hAWA-FuGmhiuMptnKNk_dU3VnbyL6SbHrMWUbquxWjyoqsd7stFs1K_nW6XIzsTjh8Bg6hB5hmsSV-M5_hPS24JwJaCdMQeWrh6cIEp2Sjft7I1V4HQrgzrkMh15sDFAw3i1_ZZasQsDYKyYbO9Jp7lx42ognPrz_KuvPzLjEXvBBNTFsVXUE-ur5adLNMvt-uXzcJ1rcwhjHWItUf5YvgRQbbBnd9f-LsJIhfkDgCJcvZmGDZrtlCKaU1UjHv5c3faZED-cjL59MbibofhPjv87MK8hhdg";
        const string TOKEN_ACCOUNT_123_TEST1_AT_TEST_DOT_COM_EXPIRE_20010908 = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOjEyMywidW5pcXVlX25hbWUiOiJ0ZXN0MUB0ZXN0LmNvbSIsInJvbGUiOiJVU0VSIiwiZXhwIjoxMDAwMDAwMDAwfQ.JBmiZBgKVSUtB4_NhD1kiUhBTnH2ufGSzcoCwC3-Gtx0QDvkFjy2KbxIU9asscenSdzziTOZN6IfFx6KgZ3_a3YB7vdCgfSINQwrAK0_6Owa-BQuNAIsKk-pNoIhJ-OcckV-zrp5wWai3Ak5Qzg3aZ1NKZQKZt5ICZmsFZcWu_4pzS-xsGPcj5gSr3Iybt61iBnetrkrEbjtVZg-3xzKr0nmMMqe-qqeknozIFy2YWAObmTkrN4sZ3AB_jzqyFPXN-nMw3a0NxIdJyetbESAOcNnPLymBKZEZmX2psKuXwJxxekvgK9egkfv2EjKYF9atpH5XwC0Pd4EWvraLAL2eg";

        private readonly WebApplicationFactory<Startup> _factory;
        private readonly ITestOutputHelper _output;

        public GetPlanInformationTest(WebApplicationFactory<Startup> factory, ITestOutputHelper output)
        {
            _factory = factory;
            _output = output;
        }

        [Fact]
        public async Task GET_plan_information_should_return_not_found_when_empty_db_result()
        {
            // Arrange
            var accountPlansRepositoryMock = new Mock<IAccountPlansRepository>();
            var integrationsRepositoryMock = new Mock<IIntegrationsRepository>();

            accountPlansRepositoryMock.Setup(x => x.GetPlanInformation("test1@test.com")).ReturnsAsync(() => null);

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton<AccountPlansService>();
                    services.AddSingleton(accountPlansRepositoryMock.Object);
                    services.AddSingleton(integrationsRepositoryMock.Object);
                });

            }).CreateClient(new WebApplicationFactoryClientOptions());

            var request = new HttpRequestMessage(HttpMethod.Get, "accounts/test1@test.com/plan")
            {
                Headers = { { "Authorization", $"Bearer {TOKEN_ACCOUNT_123_TEST1_AT_TEST_DOT_COM_EXPIRE_20330518}" } }
            };

            // Act
            var response = await client.SendAsync(request);
            _output.WriteLine(response.GetHeadersAsString());

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            accountPlansRepositoryMock.Verify(x => x.GetPlanInformation(It.IsAny<string>()), Times.Once());
            integrationsRepositoryMock.Verify(x => x.GetIntegrationsStatusByUserAccount(It.IsAny<string>()), Times.Never());
        }

        [Fact]
        public async Task GET_plan_information_should_return_right_value_based_on_db_response()
        {
            // Arrange
            var planInformationDbResponse = new PlanInformation
            {
                IdUser = 1,
                IdUserType = Enums.UserTypesEnum.Monthly,
                UserType = nameof(Enums.UserTypesEnum.Monthly),
                IdUserTypePlan = 1,
                UserTypePlan = "1500",
                PlanType = "STANDARD",
                IdIndustry = 1,
                IndustryCode = "dplr1",
                Country = "AR",
                Integrations = Array.Empty<string>(),
            };

            var expectedResponse = @"{""idUser"":1,""idUserType"":2,""userType"":""Monthly"",""idUserTypePlan"":1,""userTypePlan"":""1500"",""planType"":""STANDARD"",""idIndustry"":1,""industryCode"":""dplr1"",""country"":""AR"",""integrations"":[""DataHub"",""Dkim""]}";

            var accountPlansRepositoryMock = new Mock<IAccountPlansRepository>();
            var integrationsRepositoryMock = new Mock<IIntegrationsRepository>();

            accountPlansRepositoryMock.Setup(x => x.GetPlanInformation("test1@test.com")).ReturnsAsync(planInformationDbResponse);

            integrationsRepositoryMock.Setup(x => x.GetIntegrationsStatusByUserAccount("test1@test.com")).ReturnsAsync(new Dictionary<string, string>
            {
                { "ApiKeyStatus", "disconnected" },
                { "DataHubStatus", "connected" },
                { "DkimStatus", "connected" },
                { "ShopifyStatus", "disconnected" }
            });

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton<AccountPlansService>();
                    services.AddSingleton(accountPlansRepositoryMock.Object);
                    services.AddSingleton(integrationsRepositoryMock.Object);
                });

            }).CreateClient(new WebApplicationFactoryClientOptions());

            var request = new HttpRequestMessage(HttpMethod.Get, "accounts/test1@test.com/plan")
            {
                Headers = { { "Authorization", $"Bearer {TOKEN_ACCOUNT_123_TEST1_AT_TEST_DOT_COM_EXPIRE_20330518}" } }
            };

            // Act
            var response = await client.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            _output.WriteLine(responseContent);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(expectedResponse, responseContent);
            accountPlansRepositoryMock.Verify(x => x.GetPlanInformation(It.IsAny<string>()), Times.Once());
            integrationsRepositoryMock.Verify(x => x.GetIntegrationsStatusByUserAccount(It.IsAny<string>()), Times.Once());
        }

        [Theory]
        [InlineData(TOKEN_ACCOUNT_123_TEST1_AT_TEST_DOT_COM_EXPIRE_20010908)]
        [InlineData("")]
        [InlineData("invalid")]
        public async Task GET_plan_information_should_return_unauthorized_when_token_is_invalid(string token)
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions());

            var request = new HttpRequestMessage(HttpMethod.Get, "accounts/test1@test.com/plan")
            {
                Headers = { { "Authorization", $"Bearer {token}" } }
            };

            // Act
            var response = await client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }


        [Fact]
        public async Task GET_plan_information_should_return_unauthorized_when_authorization_is_empty()
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions());

            var request = new HttpRequestMessage(HttpMethod.Get, "accounts/test1@test.com/plan");

            // Act
            var response = await client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
