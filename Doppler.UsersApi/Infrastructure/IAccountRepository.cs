using Doppler.UsersApi.Model;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Doppler.UsersApi.Infrastructure
{
    public interface IAccountRepository
    {
        Task<ContactInformation> GetContactInformation(string accountName);
        Task UpdateContactInformation(string accountName, ContactInformation contactInformation);
        Task<List<InvitationInformation>> GetUserInvitations(string email, CancellationToken cancellationToken = default);
    }
}
