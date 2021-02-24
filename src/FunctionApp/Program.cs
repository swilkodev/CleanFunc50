using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.Functions.Worker.Configuration;
using FunctionApp.Middleware;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.FunctionApp;

namespace FunctionApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
// #if DEBUG
//             Debugger.Launch();
// #endif
            var host = new HostBuilder()
                .ConfigureAppConfiguration(c =>
                {
                    c.AddCommandLine(args);
                    c.AddJsonFile("appsettings.json", true);
                    c.AddJsonFile("local.settings.json", true);
                })
                .ConfigureFunctionsWorker((c, b) =>
                {
                    b.UseSampleMiddleware();
                    b.UseFunctionExecutionMiddleware();
                })
                .ConfigureServices((c, s) =>
                {
                    //s.AddSingleton<IHttpResponderService, DefaultHttpResponderService>();
                    s.AddSingleton<IHttpRequestProcessor, HttpRequestProcessor>();
                    s.AddSingleton<ICurrentUserService, CurrentUserService>();

                    var startup = new Startup(c.Configuration);
                    startup.ConfigureServices(s);
                })
                .Build();

            await host.RunAsync();
        }
    }

    internal class CurrentUserService : ICurrentUserService
    {
        public string UserId => "Me";
    }
}