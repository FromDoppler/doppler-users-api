using Doppler.UsersApi.Infrastructure;
using Doppler.UsersApi.Model;
using Doppler.UsersApi.Model.Enums;
using Doppler.UsersApi.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
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
        private readonly IAccountService _accountService;

        public AccountController(
            ILogger<FeatureController> logger,
            IAccountRepository accountRepository,
            IValidator<ContactInformation> validator,
            IAccountService accountService)
        {
            _logger = logger;
            _accountRepository = accountRepository;
            _validator = validator;
            _accountService = accountService;
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

        [HttpGet("/accounts/{accountName}/user-invitations")]
        public async Task<IActionResult> GetUserInvitations(string accountName, CancellationToken cancellationToken = default)
        {
            var usersInformation = await _accountRepository.GetUserInvitations(accountName);

            if (usersInformation.Any())
            {
                return new OkObjectResult(usersInformation);
            }

            return new NotFoundResult();
        }

        [HttpPost("/accounts/{accountName}/user-invitations")]
        public async Task<IActionResult> SendUserInvitation(string accountName, [FromBody] CollaborationInvitationModel invitation, CancellationToken cancellationToken = default)
        {
            try
            {
                var userInvitations = await _accountRepository.GetUserInvitations(accountName);

                var invite = userInvitations.Where(x => x.Email == invitation.Email)
                    .FirstOrDefault();

                var collaborationInvite = new CollaborationInvite
                {
                    IdUser = invite?.IdUser ?? invitation.IdUser,
                    Email = invite?.Email ?? invitation.Email,
                    CreationDate = invite?.InvitationDate ?? DateTime.UtcNow,
                    ExpirationDate = invite?.ExpirationDate ?? DateTime.UtcNow.AddDays(1),
                    InviteStatus = invite?.InvitationStatus ?? (int)InviteStatusEnum.PENDING
                };

                _accountService.SendCollaborationInvite(collaborationInvite, invite == null);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }

            return new OkObjectResult("Success");
        }
    }
}
