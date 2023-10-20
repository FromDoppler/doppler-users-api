using Doppler.UsersApi.Infrastructure;
using Doppler.UsersApi.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Doppler.UsersApi.Services
{
    public class AccountPlansService
    {
        private readonly IAccountPlansRepository _accountPlansRepository;
        private readonly IIntegrationsRepository _integrationsRepository;

        public AccountPlansService(IAccountPlansRepository accountPlansRepository, IIntegrationsRepository integrationsRepository)
        {
            _accountPlansRepository = accountPlansRepository;
            _integrationsRepository = integrationsRepository;
        }

        public async Task<PlanInformation> GetPlanInformation(string accountName)
        {
            var planInformation = await _accountPlansRepository.GetPlanInformation(accountName);

            if (planInformation == null)
            {
                throw new KeyNotFoundException("Plan not found");
            }

            var integrations = await _integrationsRepository.GetIntegrationsStatusByUserAccount(accountName);

            planInformation.Integrations = GetActiveIntegrationNames(integrations);

            return planInformation;
        }

        private string[] GetActiveIntegrationNames(Dictionary<string, string> integrations)
        {
            var integrationsList = new List<string>();
            foreach (var item in integrations)
            {
                if (item.Value.ToLower() == "connected")
                {
                    //Remove status
                    integrationsList.Add(item.Key[..^"status".Length]);
                }
            }

            return integrationsList.ToArray();
        }
    }
}
