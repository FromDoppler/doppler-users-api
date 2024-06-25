using Doppler.UsersApi.Infrastructure;
using Doppler.UsersApi.Model;
using Doppler.UsersApi.Model.Enums;
using System;

namespace Doppler.UsersApi.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;

        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public void SendCollaborationInvite(CollaborationInvite invite, bool isNewInvite)
        {
            if (isNewInvite)
            {
                _accountRepository.CreateCollaborationInvite(invite);
            }
            else
            {
                if (IsActiveInvitation(invite))
                {
                    throw new Exception("User has already an approved or pending invitation");
                }

                invite.ExpirationDate = DateTime.UtcNow.AddDays(1);
                invite.InviteStatus = (int)InviteStatusEnum.PENDING;

                _accountRepository.UpdateCollaborationInvite(invite);
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
