using Dapper;
using Doppler.UsersApi.Model;
using Doppler.UsersApi.Test.Utils;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Moq;
using Moq.Dapper;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Doppler.UsersApi.Encryption;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Doppler.UsersApi.Test
{
    public class GetContactInformationTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        const string TOKEN_ACCOUNT_123_TEST1_AT_TEST_DOT_COM_EXPIRE_20330518 = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOjEyMywidW5pcXVlX25hbWUiOiJ0ZXN0MUB0ZXN0LmNvbSIsInJvbGUiOiJVU0VSIiwiZXhwIjoyMDAwMDAwMDAwfQ.E3RHjKx9p0a-64RN2YPtlEMysGM45QBO9eATLBhtP4tUQNZnkraUr56hAWA-FuGmhiuMptnKNk_dU3VnbyL6SbHrMWUbquxWjyoqsd7stFs1K_nW6XIzsTjh8Bg6hB5hmsSV-M5_hPS24JwJaCdMQeWrh6cIEp2Sjft7I1V4HQrgzrkMh15sDFAw3i1_ZZasQsDYKyYbO9Jp7lx42ognPrz_KuvPzLjEXvBBNTFsVXUE-ur5adLNMvt-uXzcJ1rcwhjHWItUf5YvgRQbbBnd9f-LsJIhfkDgCJcvZmGDZrtlCKaU1UjHv5c3faZED-cjL59MbibofhPjv87MK8hhdg";
        const string TOKEN_ACCOUNT_123_TEST1_AT_TEST_DOT_COM_EXPIRE_20010908 = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOjEyMywidW5pcXVlX25hbWUiOiJ0ZXN0MUB0ZXN0LmNvbSIsInJvbGUiOiJVU0VSIiwiZXhwIjoxMDAwMDAwMDAwfQ.JBmiZBgKVSUtB4_NhD1kiUhBTnH2ufGSzcoCwC3-Gtx0QDvkFjy2KbxIU9asscenSdzziTOZN6IfFx6KgZ3_a3YB7vdCgfSINQwrAK0_6Owa-BQuNAIsKk-pNoIhJ-OcckV-zrp5wWai3Ak5Qzg3aZ1NKZQKZt5ICZmsFZcWu_4pzS-xsGPcj5gSr3Iybt61iBnetrkrEbjtVZg-3xzKr0nmMMqe-qqeknozIFy2YWAObmTkrN4sZ3AB_jzqyFPXN-nMw3a0NxIdJyetbESAOcNnPLymBKZEZmX2psKuXwJxxekvgK9egkfv2EjKYF9atpH5XwC0Pd4EWvraLAL2eg";

        private readonly WebApplicationFactory<Startup> _factory;
        private readonly ITestOutputHelper _output;

        public GetContactInformationTest(WebApplicationFactory<Startup> factory, ITestOutputHelper output)
        {
            _factory = factory;
            _output = output;
        }

        [Fact]
        public async Task GET_contact_information_should_return_not_found_when_empty_db_result()
        {
            // Arrange

            var mockConnection = new Mock<DbConnection>();

            // TODO: validate input
            mockConnection.SetupDapperAsync(c => c.QueryAsync<ContactInformation>(null, null, null, null, null)).ReturnsAsync(Enumerable.Empty<ContactInformation>());

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.SetupConnectionFactory(mockConnection.Object);
                });

            }).CreateClient(new WebApplicationFactoryClientOptions());

            var request = new HttpRequestMessage(HttpMethod.Get, "accounts/test1@test.com/contact-information")
            {
                Headers = { { "Authorization", $"Bearer {TOKEN_ACCOUNT_123_TEST1_AT_TEST_DOT_COM_EXPIRE_20330518}" } }
            };

            // Act
            var response = await client.SendAsync(request);
            _output.WriteLine(response.GetHeadersAsString());

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Theory]
        [InlineData(TOKEN_ACCOUNT_123_TEST1_AT_TEST_DOT_COM_EXPIRE_20010908)]
        [InlineData("")]
        [InlineData("invalid")]
        public async Task GET_contact_information_should_return_unauthorized_when_token_is_invalid(string token)
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions());

            var request = new HttpRequestMessage(HttpMethod.Get, "accounts/test1@test.com/contact-information")
            {
                Headers = { { "Authorization", $"Bearer {token}" } }
            };

            // Act
            var response = await client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GET_contact_information_should_return_unauthorized_when_authorization_is_empty()
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions());

            var request = new HttpRequestMessage(HttpMethod.Get, "accounts/test1@test.com/contact-information");

            // Act
            var response = await client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GET_contact_information_should_return_right_value_based_on_db_response()
        {
            // Arrange
            var dbResponse = new ContactInformation
            {
                Firstname = "Test First Name",
                Lastname = "Test Last Name",
                Email = "test1@test.com",
                Phone = "5555555",
                Company = "Test Company",
                Address = "Test Address",
                City = "Test City",
                Province = "Test Province",
                ZipCode = "Test ZipCode",
                Country = "Test Country",
                Industry = "Industry Test",
                AnswerSecurityQuestion = "test",
                IdSecurityQuestion = "1"
            };

            const string expectedContent = "{\"email\":\"test1@test.com\",\"firstname\":\"Test First Name\",\"lastname\":\"Test Last Name\",\"address\":\"Test Address\",\"city\":\"Test City\",\"province\":\"Test Province\",\"country\":\"Test Country\",\"zipCode\":\"Test ZipCode\",\"phone\":\"5555555\",\"company\":\"Test Company\",\"industry\":\"Industry Test\",\"answerSecurityQuestion\":\"TEST\",\"idSecurityQuestion\":\"1\"}";

            var mockConnection = new Mock<DbConnection>();

            // TODO: validate input
            mockConnection.SetupDapperAsync(c => c.QueryFirstOrDefaultAsync<ContactInformation>(null, null, null, null, null))
                .ReturnsAsync(dbResponse);

            var encryptedMock = new Mock<IEncryptionService>();
            encryptedMock.Setup(x => x.DecryptAES256(It.IsAny<string>())).Returns("TEST");

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.SetupConnectionFactory(mockConnection.Object);
                    services.AddSingleton(encryptedMock.Object);
                });

            }).CreateClient(new WebApplicationFactoryClientOptions());

            var request = new HttpRequestMessage(HttpMethod.Get, "accounts/test1@test.com/contact-information")
            {
                Headers = { { "Authorization", $"Bearer {TOKEN_ACCOUNT_123_TEST1_AT_TEST_DOT_COM_EXPIRE_20330518}" } }
            };

            // Act
            var response = await client.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            _output.WriteLine(responseContent);

            // Assert
            Assert.Equal(expectedContent, responseContent);
        }
    }
}
