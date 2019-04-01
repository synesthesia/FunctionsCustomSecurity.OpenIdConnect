using System.Security.Claims;
using System.Threading.Tasks;
using FunctionsCustomSecurity.OpenIdConnect.Binding;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace FunctionsCustomSecurity.OpenIdConnect
{
    public static class ExampleHttpFunction
    {
        [FunctionName("ExampleHttpFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "example")] HttpRequest req, 
            ILogger log, 
            [AccessToken] ClaimsPrincipal principal)
        {
            log.LogInformation($"Request received for {principal.Identity.Name}.");
            return new OkResult();
        }
    }
}
