using Newtonsoft.Json;
using System;

namespace Doppler.UsersApi.Model
{
    public class CollaborationInvite
    {
        public int IdUser { get; set; }
        public string Email { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public int InviteStatus { get; set; }
    }
}
