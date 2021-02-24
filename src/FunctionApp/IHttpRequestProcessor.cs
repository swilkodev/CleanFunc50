using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Pipeline;

namespace CleanArchitecture.FunctionApp
{
    public interface IHttpRequestProcessor
    {
        Task<HttpResponseData> ExecuteAsync<TRequest, TResponse>(FunctionExecutionContext executionContext, 
                                                                                HttpRequestData httpRequest,
                                                                                TRequest request, 
                                                                                Func<TResponse, HttpResponseData> resultMethod = null)
                                                                                where TRequest : IRequest<TResponse>;

        Task<TResponse> ExecuteAsync<TRequest, TResponse>(FunctionExecutionContext executionContext, 
                                                                                HttpRequestData httpRequest,
                                                                                TRequest request)
                                                                                where TRequest : IRequest<TResponse>;
    }
}