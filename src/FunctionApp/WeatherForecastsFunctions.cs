using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using CleanArchitecture.Application.WeatherForecasts.Queries.GetWeatherForecasts;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Pipeline;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CleanArchitecture.FunctionApp
{
    public class WeatherForecastsFunctions
    {
        private readonly IHttpRequestProcessor mediator;

        public WeatherForecastsFunctions(IHttpRequestProcessor mediator)
        {
            this.mediator = mediator;
        }

        [FunctionName(nameof(GetWeatherForecasts))]
        public async Task<IEnumerable<WeatherForecast>> GetWeatherForecasts([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "weather/forecasts")] HttpRequestData req,
            FunctionExecutionContext executionContext)
        {
            var logger = executionContext.Logger;
            logger.LogInformation("Called GetWeatherForecasts");

            return await this.mediator.ExecuteAsync<GetWeatherForecastsQuery, IEnumerable<WeatherForecast>>(executionContext, 
                                                                req, 
                                                                new GetWeatherForecastsQuery());
        }
    }
}