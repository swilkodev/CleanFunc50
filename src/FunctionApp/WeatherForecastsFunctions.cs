using System.Collections.Generic;
using System.Threading.Tasks;
using CleanArchitecture.Application.WeatherForecasts.Queries.GetWeatherForecasts;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.FunctionApp
{
    public class WeatherForecastsFunctions
    {
        private readonly IHttpRequestProcessor mediator;

        public WeatherForecastsFunctions(IHttpRequestProcessor mediator)
        {
            this.mediator = mediator;
        }

        [Function(nameof(GetWeatherForecasts))]
        public async Task<IEnumerable<WeatherForecast>> GetWeatherForecasts([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "weather/forecasts")] HttpRequestData req,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger<WeatherForecastsFunctions>();
            logger.LogInformation("Called GetWeatherForecasts");

            return await this.mediator.ExecuteAsync<GetWeatherForecastsQuery, IEnumerable<WeatherForecast>>(executionContext, 
                                                                req, 
                                                                new GetWeatherForecastsQuery());
        }
    }
}