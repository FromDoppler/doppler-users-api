using System.Threading.Tasks;
using Doppler.UsersApi.DopplerSecurity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Microsoft.Extensions.Logging;
using Doppler.UsersApi.Infrastructure;
using Doppler.UsersApi.Model;

namespace Doppler.UsersApi.Controllers
{
    [Authorize]
    [ApiController]
    public class ReportController
    {
        private readonly ILogger _logger;

        public ReportController(ILogger<ReportController> logger, IFeaturesRepository featuresRepository)
        {
            _logger = logger;
        }

        [Authorize(Policies.OWN_RESOURCE_OR_SUPERUSER)]
        [HttpGet("/accounts/{accountName}/reports/subscribers/summary")]
        public async Task<IActionResult> GetSubscribersSummary(string accountName)
        {

            var subscribersSummary = new SubscribersSummary { NewSubscribers = 111, RemovedSubscribers = 22, TotalSubscribers = 333 };
            return new OkObjectResult(subscribersSummary);
        }

        [Authorize(Policies.OWN_RESOURCE_OR_SUPERUSER)]
        [HttpGet("/accounts/{accountName}/reports/subscribers/summary")]
        public async Task<IActionResult> GetCampaignsSummary(string accountName)
        {

            var campaignsSummary = new CampaignsSummary { ClickThroughRate = 1.34M, TotalOpensClicks = 22, TotalSendedEmails = 333 };
            return new OkObjectResult(campaignsSummary);
        }
    }
}
