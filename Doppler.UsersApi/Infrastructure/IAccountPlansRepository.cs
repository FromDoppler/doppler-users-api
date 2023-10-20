using Doppler.UsersApi.Model;
using System.Threading.Tasks;

namespace Doppler.UsersApi.Infrastructure
{
    public interface IAccountPlansRepository
    {
        Task<PlanInformation> GetPlanInformation(string accountName);
    }
}
