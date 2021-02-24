using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Pipeline;

namespace CleanArchitecture.FunctionApp
{
    public class HttpRequestProcessor : IHttpRequestProcessor
    {
        private readonly IMediator mediator;

        public HttpRequestProcessor(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task<HttpResponseData> ExecuteAsync<TRequest, TResponse>(FunctionExecutionContext executionContext, HttpRequestData httpRequest, TRequest request, Func<TResponse, HttpResponseData> resultMethod = null)
            where TRequest : IRequest<TResponse>
        {
            // TODO 
            // Add logic to extract info from HttpRequestData
            // Add logic to return problemdetails structures on any exception
            var response = await mediator.Send(request);

            return resultMethod(response);
        }

        public async Task<TResponse> ExecuteAsync<TRequest, TResponse>(FunctionExecutionContext executionContext, HttpRequestData httpRequest, TRequest request) where TRequest : IRequest<TResponse>
        {
            return await mediator.Send(request);
        }
    }
}