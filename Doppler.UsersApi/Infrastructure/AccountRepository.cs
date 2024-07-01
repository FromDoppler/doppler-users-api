using Dapper;
using Doppler.UsersApi.Model;
using System.Threading.Tasks;
using Doppler.UsersApi.Encryption;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Doppler.UsersApi.Infrastructure
{
    public class AccountRepository : IAccountRepository
    {
        private const string DateFormat = "yyyy-MM-dd HH:mm:ss";
        private readonly IDatabaseConnectionFactory _connectionFactory;
        private readonly IEncryptionService _encryptionService;
        public AccountRepository(IDatabaseConnectionFactory connectionFactory, IEncryptionService encryptionService)
        {
            _connectionFactory = connectionFactory;
            _encryptionService = encryptionService;
        }
        public async Task<ContactInformation> GetContactInformation(string email)
        {
            using var connection = _connectionFactory.GetConnection();
            var result = await connection.QueryFirstOrDefaultAsync<ContactInformation>(@"
SELECT
    U.FirstName,
    U.LastName,
    U.Email,
    isnull(I.Code, '') AS Industry,
    U.Company, U.PhoneNumber AS Phone,
    U.Address, U.ZipCode,
    U.CityName AS City,
    isnull(S.StateCode, '') AS Province,
    isnull(CO.Code, '') AS Country,
    U.[AnswerSecurityQuestion],
    U.[IdSecurityQuestion]
FROM
    [User] U
    LEFT JOIN [State] S ON U.IdState = S.IdState
    LEFT JOIN [Country] CO ON S.IdCountry = CO.IdCountry
    LEFT JOIN [Industry] I ON I.IdIndustry = U.IdIndustry
WHERE
    U.Email = @email",
                new { email });

            if (result != null)
            {
                result.AnswerSecurityQuestion = _encryptionService.DecryptAES256(result.AnswerSecurityQuestion);
            }

            return result;
        }

        public async Task UpdateContactInformation(string accountName, ContactInformation contactInformation)
        {
            using var connection = _connectionFactory.GetConnection();
            //Update User
            await connection.ExecuteAsync(@"
UPDATE [User] SET
    FirstName = @firstname,
    LastName = @lastname,
    IdIndustry = (SELECT IdIndustry FROM [Industry] WHERE Code = @industrycode),
    Company = @company,
    PhoneNumber = @phonenumber,
    Address = @address,
    ZipCode = @zipcode,
    CityName = @cityname,
    IdState = ISNULL((SELECT IdState FROM [State] WHERE StateCode = @province AND CountryCode = @country), (SELECT IdState FROM [State] WHERE StateCode = 'NO-DEF' AND CountryCode = @country)),
    IdSecurityQuestion = @idsecurityquestion,
    AnswerSecurityQuestion = @encryptedanswer
WHERE
    Email = @email;",
                new
                {
                    @firstname = contactInformation.Firstname,
                    @lastname = contactInformation.Lastname,
                    @industrycode = contactInformation.Industry,
                    @company = contactInformation.Company,
                    @phonenumber = contactInformation.Phone,
                    @address = contactInformation.Address,
                    @zipcode = contactInformation.ZipCode,
                    @cityname = contactInformation.City,
                    @province = contactInformation.Province,
                    @country = contactInformation.Country,
                    @idsecurityquestion = contactInformation.IdSecurityQuestion,
                    @encryptedanswer = contactInformation.AnswerSecurityQuestion == null ? "" : _encryptionService.EncryptAES256(contactInformation.AnswerSecurityQuestion.ToUpper()),
                    @email = accountName
                });
        }

        public async Task<List<InvitationInformation>> GetUserInvitations(string email, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.GetConnection();
            var result = await connection.QueryAsync<InvitationInformation>(@"
SELECT  ci.IdUser,
    ci.Email,
    ua.FirstName,
    ua.LastName,
    ci.UTCCreationDate AS InvitationDate,
    ci.UTCExpirationDate AS ExpirationDate,
    ci.InviteStatus AS InvitationStatus
FROM [dbo].[User] u
JOIN [dbo].[ColaborationInvites] ci
    ON ci.iduser = u.idUser
LEFT JOIN [dbo].[UserAccount] ua
    ON ci.IdUserAccount = ua.IdUserAccount
WHERE u.Email = @email",
            new { email });

            return result.ToList();
        }

        public async Task<int> CreateCollaborationInvite(CollaborationInvite invite)
        {
            using var connection = _connectionFactory.GetConnection();

            return await connection.ExecuteAsync(@"
INSERT INTO [dbo].[ColaborationInvites]
VALUES(@idUser, @email, NULL, @creationDate, @expirationDate, NULL, @status,0)",
            new
            {
                @idUser = invite.IdUser,
                @email = invite.Email,
                @creationDate = invite.CreationDate.ToString(DateFormat),
                @expirationDate = invite.ExpirationDate.ToString(DateFormat),
                @status = invite.InviteStatus,
            });
        }

        public async Task<int> UpdateCollaborationInvite(CollaborationInvite invite)
        {
            using var connection = _connectionFactory.GetConnection();

            return await connection.ExecuteAsync(@"
UPDATE [dbo].[ColaborationInvites]
SET UTCExpirationDate = @expirationDate,
    InviteStatus = @status
WHERE IdUser = @idUser
    AND Email = @email",
            new
            {
                @idUser = invite.IdUser,
                @email = invite.Email,
                @expirationDate = invite.ExpirationDate.ToString(DateFormat),
                @status = invite.InviteStatus,
            });
        }
    }
}
