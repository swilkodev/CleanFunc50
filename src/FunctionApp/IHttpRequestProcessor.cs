using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace CleanArchitecture.FunctionApp
{
    public interface IHttpRequestProcessor
    {
        Task<HttpResponseData> ExecuteAsync<TRequest, TResponse>(FunctionContext executionContext, 
                                                                                HttpRequestData httpRequest,
                                                                                TRequest request, 
                                                                                Func<TResponse, Task<HttpResponseData>> resultMethod = null)
                                                                                where TRequest : IRequest<TResponse>;

        Task<TResponse> ExecuteAsync<TRequest, TResponse>(FunctionContext executionContext, 
                                                                                HttpRequestData httpRequest,
                                                                                TRequest request)
                                                                                where TRequest : IRequest<TResponse>;
    }
}