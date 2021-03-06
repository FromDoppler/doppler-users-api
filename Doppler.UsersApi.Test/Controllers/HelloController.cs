using Doppler.UsersApi.DopplerSecurity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Doppler.UsersApi.Test.Controllers
{
    [Authorize]
    [ApiController]
    public class HelloController
    {
        [AllowAnonymous]
        [HttpGet("/hello/anonymous")]
        public string GetForAnonymous()
        {
            return "Hello anonymous!";
        }

        [HttpGet("/hello/valid-token")]
        public string GetForValidToken()
        {
            return "Hello! you have a valid token!";
        }

        [Authorize(Policies.ONLY_SUPERUSER)]
        [HttpGet("/hello/superuser")]
        public string GetForSuperUserToken()
        {
            return "Hello! you have a valid SuperUser token!";
        }

        [Authorize(Policies.OWN_RESOURCE_OR_SUPERUSER)]
        [HttpGet("/accounts/{accountId:int:min(0)}/hello")]
        public string GetForAccountById(int accountId)
        {
            return $"Hello! \"you\" that have access to the account with ID '{accountId}'";
        }

        [Authorize(Policies.OWN_RESOURCE_OR_SUPERUSER)]
        [HttpGet("/accounts/{accountName}/hello")]
        public string GetForAccountByName(string accountName)
        {
            return $"Hello! \"you\" that have access to the account with accountName '{accountName}'";
        }
    }
}
