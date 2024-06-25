using System;

namespace Doppler.UsersApi.Model
{
    public class InvitationInformation
    {
        public int IdUser { get; set; }
        public string Email { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public DateTime InvitationDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public int InvitationStatus { get; set; }
    }
}
