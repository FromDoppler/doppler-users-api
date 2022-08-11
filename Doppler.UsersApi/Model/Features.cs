using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doppler.UsersApi.Model
{
    public class Features
    {
        [Obsolete("This feature is available for all users. Consider remotion related to [this ticket](https://makingsense.atlassian.net/browse/DOP-1096)")]
        public bool ContactPolicies { get; } = true;

        public bool BigQuery { get; set; }

        public bool SmartCampaigns { get; set; }

        public bool SmartCampaingsExtraCustomizations { get; set; }

        public bool SmartSubjectCampaigns { get; set; }

        public bool EmailParameter { get; set; }

        public bool SiteTracking { get; set; }

        public bool BmwCrmIntegration { get; set; }
    }
}
