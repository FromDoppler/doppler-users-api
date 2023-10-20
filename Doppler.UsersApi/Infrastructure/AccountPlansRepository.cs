using Dapper;
using Doppler.UsersApi.Model;
using System.Threading.Tasks;

namespace Doppler.UsersApi.Infrastructure
{
    public class AccountPlansRepository : IAccountPlansRepository
    {
        private readonly IDatabaseConnectionFactory _connectionFactory;

        public AccountPlansRepository(IDatabaseConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<PlanInformation> GetPlanInformation(string accountName)
        {
            using var connection = _connectionFactory.GetConnection();

            var planBillingInformation = await connection.QueryFirstOrDefaultAsync<PlanInformation>(@"
SELECT
    U.IdUser,
    U.Email,
    UT.IdUserType,
    UT.[Description] as UserType,
    UTP.IdUserTypePlan,
    UTP.[Description] as UserTypePlan,
    UTP.[PlanType],
    U.IdIndustry,
    I.Code as IndustryCode,
    isnull(CO.Code, '') AS Country
FROM [dbo].[BillingCredits] BC
INNER JOIN [User] U ON U.IdUser = BC.IdUser
INNER JOIN [UserTypesPlans] UTP ON UTP.IdUserTypePlan = BC.IdUserTypePlan
INNER JOIN [UserTypes] UT ON UT.IdUserType = UTP.IdUserType
INNER JOIN [State] S ON U.IdBillingState = S.IdState
INNER JOIN [Country] CO ON S.IdCountry = CO.IdCountry
INNER JOIN [Industry] I ON I.IdIndustry = U.IdIndustry
WHERE U.Email = @email
ORDER BY BC.[ActivationDate] DESC;",
                new
                {
                    @email = accountName
                });

            return planBillingInformation;
        }
    }
}
