using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;

namespace FunctionApp.Middleware
{
    public static class ApplicationBuilderExtensions
    {
        public static IFunctionsWorkerApplicationBuilder UseSampleMiddleware(this IFunctionsWorkerApplicationBuilder builder)
        {
            builder.Services.AddSingleton<SampleMiddleware>();

            builder.Use(next =>
            {
                return context =>
                {
                    var middleware = context.InstanceServices.GetRequiredService<SampleMiddleware>();

                    return middleware.Invoke(context, next);
                };
            });

            return builder;
        }
    }
}