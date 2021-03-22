using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Model;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CircuitBreaker.Client.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
        private readonly string _baseUrl = "";
        public HomeController()
        {
            _retryPolicy = Policy.HandleResult<HttpResponseMessage>(result =>
              result.IsSuccessStatusCode).RetryAsync(5, (a, b) => {
                  string c = "Retry";
              });
        }

        public async Task< IActionResult> Index()
        {
            HttpClient client = new HttpClient();
            var result =await _retryPolicy.ExecuteAsync(()=> client.GetAsync(_baseUrl)) ;
            var str = await result.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeObject<MessageClient>(str);
            return Ok(obj);
        }
    }
}
