using Doppler.UsersApi.Model;

namespace Doppler.UsersApi.Services
{
    public interface IAccountService
    {
        void SendCollaborationInvite(CollaborationInvite invite, bool isNewInvite);
    }
}
