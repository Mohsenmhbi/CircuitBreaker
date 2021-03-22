using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Model;
using Newtonsoft.Json;
using Polly;
using Polly.CircuitBreaker;
using Polly.Fallback;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;

namespace CircuitBreaker.Client.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
        private static AsyncCircuitBreakerPolicy<HttpResponseMessage> _circuitBreakerPolicy;
        private readonly AsyncFallbackPolicy<HttpResponseMessage> _fallbackPolicy;
        private readonly string _baseUrl = "https://localhost:44324/Home/Ten";
        private ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _retryPolicy = Policy.HandleResult<HttpResponseMessage>(result =>
              !result.IsSuccessStatusCode).RetryAsync(5, (a, b) => {
                  _logger.LogInformation("Retry");
              });

            _circuitBreakerPolicy=Policy.HandleResult<HttpResponseMessage>
                (r=>!r.IsSuccessStatusCode).Or<HttpRequestException>()
                .CircuitBreakerAsync(2,TimeSpan.FromSeconds(10),
                (d,c)=>{
                    string a= "Break";
                },
                () => {
                   string a= "Rest";

                          }, 
                () => {
                   string a= "Half";

               }
               );


            _fallbackPolicy = Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .Or<BrokenCircuitException>()
                .FallbackAsync(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {

                    Content = new ObjectContent(typeof(Message), new Message
                    {
                        Id = 100,
                        Text = "Default Text"
                    }, new JsonMediaTypeFormatter())
                });



        }

        public async Task< IActionResult> Index()
        {
            HttpClient client = new HttpClient();
            var result = await _fallbackPolicy.ExecuteAsync(() => _retryPolicy.ExecuteAsync(()
                => _circuitBreakerPolicy.ExecuteAsync(() => client.GetAsync(_baseUrl))));
            var str = await result.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeObject<MessageClient>(str);
            return Ok(obj);
        }
    }
}
