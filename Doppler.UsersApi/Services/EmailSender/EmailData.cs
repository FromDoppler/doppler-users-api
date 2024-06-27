using System.ComponentModel.DataAnnotations;

namespace Doppler.UsersApi.Services.EmailSender
{
    public class EmailData
    {
        [EmailAddress]
        public string RecipientEmail { get; set; }
        public string RecipientName { get; set; }
        public string Subject { get; set; }
        public string HtmlBody { get; set; }
        public string TextBody { get; set; }
        public string TemplateId { get; set; }
        public object TemplateModel { get; set; }
    }
}
