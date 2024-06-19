using Newtonsoft.Json;
using System;

namespace Doppler.UsersApi.Model
{
    public class BasicUserInformation
    {
        public string Email { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public DateTime? InvitationDate { get; set; }
        public int? InvitationStatus { get; set; }
    }
}
