using Doppler.UsersApi.Infrastructure;
using Doppler.UsersApi.Model;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Doppler.UsersApi.Controllers
{
    [Authorize]
    [ApiController]
    public class AccountController
    {
        private readonly ILogger _logger;
        private readonly IAccountRepository _accountRepository;
        private readonly IValidator<ContactInformation> _validator;

        public AccountController(
            ILogger<FeatureController> logger,
            IAccountRepository accountRepository,
            IValidator<ContactInformation> validator)
        {
            _logger = logger;
            _accountRepository = accountRepository;
            _validator = validator;
        }

        [HttpGet("/accounts/{accountName}/contact-information")]
        public async Task<IActionResult> GetContactInformation(string accountName)
        {
            var contactInformation = await _accountRepository.GetContactInformation(accountName);

            if (contactInformation == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(contactInformation);
        }

        [HttpPut("/accounts/{accountName}/contact-information")]
        public async Task<IActionResult> UpdateContactInformation(string accountName, [FromBody] ContactInformation contactInformation)
        {
            _logger.LogDebug("Updating Contact Information.");

            var results = await _validator.ValidateAsync(contactInformation);
            if (!results.IsValid)
            {
                return new BadRequestObjectResult(results.ToString("-"));
            }

            await _accountRepository.UpdateContactInformation(accountName, contactInformation);

            return new OkObjectResult("Successfully");
        }

        [HttpGet("/accounts/{accountName}/related-users/{userType}")]
        public async Task<IActionResult> GetRelatedUsersByType(string accountName, int userType)
        {
            var usersInformation = await _accountRepository.GetRelatedUsers(accountName, userType);

            if (usersInformation == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(usersInformation);
        }
    }
}
