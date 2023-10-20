using Doppler.UsersApi.Infrastructure;
using Doppler.UsersApi.Model;
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
        private readonly IAccountPlansRepository _accountPlansRepository;
        private readonly IIntegrationsRepository _integrationsRepository;

        public AccountController(
            ILogger<FeatureController> logger,
            IAccountRepository accountRepository,
            IValidator<ContactInformation> validator,
            IAccountPlansRepository accountPlansRepository,
            IIntegrationsRepository integrationsRepository)
        {
            _logger = logger;
            _accountRepository = accountRepository;
            _validator = validator;
            _accountPlansRepository = accountPlansRepository;
            _integrationsRepository = integrationsRepository;
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
            var planInformation = await _accountPlansRepository.GetPlanInformation(accountName);

            if (planInformation == null)
            {
                return new NotFoundResult();
            }

            var integrations = await _integrationsRepository.GetIntegrationsStatusByUserAccount(accountName);

            planInformation.Integrations = GetActiveIntegrationNames(integrations);

            return new OkObjectResult(planInformation);
        }

        private string[] GetActiveIntegrationNames(Dictionary<string, string> integrations)
        {
            var integrationsList = new List<string>();
            foreach (var item in integrations)
            {
                if (item.Value.ToLower() == "connected")
                {
                    //Remove status
                    integrationsList.Add(item.Key[..^"status".Length]);
                }
            }

            return integrationsList.ToArray();
        }
    }
}
