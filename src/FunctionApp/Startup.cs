using CleanArchitecture.Application;
using CleanArchitecture.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.FunctionApp
{
    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection collection)
        {
            collection.AddApplication();

            // TODO The Azure Function should not have any dependency on Infrastructure other than DI
            // Determine a better technique
            collection.AddInfrastructure(this.configuration);
        }
    }
}