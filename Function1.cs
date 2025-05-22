using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;


namespace BasicHttpTrigger
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;
        

        public Function1(ILogger<Function1> logger)
        {
            _logger = logger;
        }

        [Function("Function1")]
        public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);

            string name = query["name"];

            if (string.IsNullOrEmpty(name))
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);

                name = data?.name;
            }

            string responseMessage = string.IsNullOrEmpty(name)
                ? "Please provide a name in the query string or request body."
                : $"Hello, {name}! Your Azure Function executed successfully.";

            var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
            await response.WriteStringAsync(responseMessage);
            return response;
        }
    }
}

