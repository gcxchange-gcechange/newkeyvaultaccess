using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Graph;

namespace newkeyvaultaccess
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<User> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string userid = req.Query["userid"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            userid = userid ?? data?.userid;

            Auth auth = new Auth();
            var graphAPIAuth = auth.graphAuth(log);

            var getInfo = await getuserInfo(graphAPIAuth,userid, log);

            return getInfo;
        }

        public static async Task<User> getuserInfo(GraphServiceClient graphServiceClient, string userid, ILogger log)
        {
            // Get user that never sign in
            var users = await graphServiceClient.Users[userid]
            .Request()
            .Select("displayName,givenName,postalCode,identities")
            .GetAsync();

            return users;
        }
    }
}
