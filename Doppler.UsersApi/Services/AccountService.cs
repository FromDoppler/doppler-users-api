using Doppler.UsersApi.Infrastructure;
using Doppler.UsersApi.Model;
using Doppler.UsersApi.Model.Enums;
using Microsoft.Extensions.Options;
using System;
using System.Reflection.Metadata.Ecma335;

namespace Doppler.UsersApi.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly AccountServiceSettings _settings;

        public AccountService(IAccountRepository accountRepository, IOptions<AccountServiceSettings> settings)
        {
            _accountRepository = accountRepository;
            _settings = settings.Value;
        }

        public void SendCollaborationInvite(CollaborationInvite invite, bool isNewInvite)
        {
            if (isNewInvite)
            {
                invite.ExpirationDate = DateTime.UtcNow.AddSeconds(_settings.InvitationExpirationSeconds);
                var response = _accountRepository.CreateCollaborationInvite(invite);

                if (response.Result == 0)
                {
                    throw new Exception("Failed to create invitation.");
                }
            }
            else
            {
                if (IsActiveInvitation(invite))
                {
                    throw new Exception("User has already an approved or pending invitation");
                }

                invite.ExpirationDate = DateTime.UtcNow.AddSeconds(_settings.InvitationExpirationSeconds);
                invite.InviteStatus = (int)InviteStatusEnum.PENDING;

                var response = _accountRepository.UpdateCollaborationInvite(invite);

                if (response.Result == 0)
                {
                    throw new Exception("Failed to update invitation.");
                }
            }

            //TODO: SendEmail(invite.Email);
        }

        private static bool IsActiveInvitation(CollaborationInvite invite)
        {
            return invite.InviteStatus == (int)InviteStatusEnum.APPROVED
                || (invite.InviteStatus == (int)InviteStatusEnum.PENDING
                    && invite.ExpirationDate > DateTime.UtcNow);
        }
    }
}
