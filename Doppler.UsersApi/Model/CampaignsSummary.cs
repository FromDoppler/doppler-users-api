namespace Doppler.UsersApi.Model
{
    public class CampaignsSummary
    {
        public int TotalSendedEmails { get; set; }

        public int TotalOpensClicks { get; set; }

        public decimal ClickThroughRate { get; set; }
    }
}
