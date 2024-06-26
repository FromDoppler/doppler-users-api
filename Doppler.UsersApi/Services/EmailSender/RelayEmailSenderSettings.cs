using System;

namespace Doppler.UsersApi.Services.EmailSender
{
    public class RelayEmailSenderSettings
    {
        public Uri UrlBase { get; set; }
        public string ApiKey { get; set; }
        public int AccountId { get; set; }
        public string FromEmail { get; set; }
        public string FromName { get; set; }
    }
}
