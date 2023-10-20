using Doppler.UsersApi.Enums;
using System.Collections.Generic;

namespace Doppler.UsersApi.Model
{
    public class PlanInformation
    {
        public int IdUser { get; set; }
        public UserTypesEnum IdUserType { get; set; }
        public string UserType { get; set; }
        public int IdUserTypePlan { get; set; }
        public string UserTypePlan { get; set; }
        public string PlanType { get; set; }
        public int IdIndustry { get; set; }

        private string _industryCode;
        public string IndustryCode
        {
            get => _industryCode;
            set => _industryCode = value.Trim();
        }
        public string Country { get; set; }

        public string[] Integrations { get; set; }
    }
}
