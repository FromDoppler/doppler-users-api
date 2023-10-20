using Doppler.UsersApi.Infrastructure;
using Doppler.UsersApi.Model;
using Doppler.UsersApi.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
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
        private readonly AccountPlansService _accountPlansService;

        public AccountController(
            ILogger<FeatureController> logger,
            IAccountRepository accountRepository,
            IValidator<ContactInformation> validator,
            AccountPlansService accountPlansService)
        {
            _logger = logger;
            _accountRepository = accountRepository;
            _validator = validator;
            _accountPlansService = accountPlansService;
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

        [HttpGet("/accounts/{accountName}/plan")]
        public async Task<IActionResult> GetPlanInformation([FromRoute] string accountName)
        {
            try
            {
                var planInformation = await _accountPlansService.GetPlanInformation(accountName);

                return new OkObjectResult(planInformation);
            }
            catch (KeyNotFoundException)
            {
                return new NotFoundResult();
            }
        }
    }
}
