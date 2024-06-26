using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Doppler.UsersApi.Services.EmailSender
{
    public class RelayEmailSender : IEmailSender
    {
        private readonly RelayEmailSenderSettings _settings;

        private readonly ILogger<RelayEmailSender> _logger;

        public RelayEmailSender(
            ILogger<RelayEmailSender> logger,
            IOptions<RelayEmailSenderSettings> options
            )
        {
            _logger = logger;
            _settings = options.Value;
        }

        private static readonly JsonSerializerOptions jsonSerializerOptions = new()
        {
            Converters = { new JsonStringEnumConverter() },
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
            WriteIndented = true,
        };

        public async Task SendEmailAsync(EmailData emailData, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(emailData.RecipientEmail, nameof(emailData.RecipientEmail));
            ArgumentNullException.ThrowIfNull(emailData.Subject, nameof(emailData.Subject));

            var requestBody = new
            {
                from_email = _settings.FromEmail,
                from_name = _settings.FromName,
                recipients = new[]
                {
                    new
                    {
                        type= "to",
                        email= emailData.RecipientEmail,
                        name= emailData.RecipientName,
                    }
                },
                subject = emailData.Subject,
                html = emailData.HtmlBody,
                text = emailData.TextBody,
            };

            try
            {
                var response = await _settings.UrlBase
                    .AppendPathSegment($"/accounts/{_settings.AccountId}/messages")
                    .WithOAuthBearerToken(_settings.ApiKey)
                    .SendJsonAsync(HttpMethod.Post, requestBody, cancellationToken: cancellationToken);
            }
            catch (FlurlHttpException ex)
            {
                _logger.LogWarning(
                    "Failed to send email to {Email}. Invoke endpoint {Url} responded {StatusCode}",
                    emailData.RecipientEmail,
                    ex.Call.Request.Url,
                    ex.StatusCode);
            }
        }

        public async Task SendTemplatedEmailAsync(EmailData emailData, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(emailData.RecipientEmail, nameof(emailData.RecipientEmail));
            ArgumentNullException.ThrowIfNull(emailData.TemplateId, nameof(emailData.TemplateId));

            var requestBody = new
            {
                from_email = _settings.FromEmail,
                from_name = _settings.FromName,
                recipients = new[]
                {
                    new
                    {
                        type= "to",
                        email= emailData.RecipientEmail,
                        name= emailData.RecipientName,
                    }
                },
                model = emailData.TemplateModel,
            };

            try
            {
                await _settings.UrlBase
                        .AppendPathSegment($"/accounts/{_settings.AccountId}/templates/{emailData.TemplateId}/message")
                        .WithOAuthBearerToken(_settings.ApiKey)
                        .SendJsonAsync(HttpMethod.Post, requestBody, cancellationToken: cancellationToken);
            }
            catch (FlurlHttpException ex)
            {
                _logger.LogWarning(
                    "Failed to send templated email to {Email}. Invoke endpoint {Url} responded {StatusCode}",
                    emailData.RecipientEmail,
                    ex.Call.Request.Url,
                    ex.StatusCode);
            }
        }
    }
}
