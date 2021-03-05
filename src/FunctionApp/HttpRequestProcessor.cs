using System;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Exceptions;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace CleanArchitecture.FunctionApp
{
    public class HttpRequestProcessor : IHttpRequestProcessor
    {
        private readonly IMediator mediator;

        public HttpRequestProcessor(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task<HttpResponseData> ExecuteAsync<TRequest, TResponse>(FunctionContext functionContext, HttpRequestData httpRequest, TRequest request, Func<TResponse, Task<HttpResponseData>> resultMethod = null)
            where TRequest : IRequest<TResponse>
        {
            // TODO 
            // Add logic to extract info from HttpRequestData
            // Add logic to return problemdetails structures on any exception

            try
            {
                var response = await mediator.Send(request);
                if(resultMethod != null)
                    return await resultMethod(response);
                
                return httpRequest.CreateResponse();
            }
            catch (NotFoundException ex)
            {
                var response = httpRequest.CreateResponse(System.Net.HttpStatusCode.NotFound);
                await response.WriteStringAsync(ex.Message);
                return response;
            }
            catch (ValidationException ex)
            {
                var response = httpRequest.CreateResponse(System.Net.HttpStatusCode.BadRequest);
                await response.WriteStringAsync(ex.Message);
                foreach(var e in ex.Errors)
                    await response.WriteStringAsync($"\r - {e.Key}:{string.Join(',',e.Value)}");
                return response;
            }
        }
    }
}