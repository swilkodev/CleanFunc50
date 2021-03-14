using System;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;

namespace CleanArchitecture.FunctionApp
{
    public static class Extensions
    {
        public static Task<HttpResponseData> CreateResponseAsync(this HttpRequestData request, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            return Task.FromResult(request.CreateResponse(statusCode));
        }

        public static Task<HttpResponseData> CreateFileContentResponseAsync(this HttpRequestData request, byte[] content, string contentType, string filename)
        {
            var contentDisposition = new Microsoft.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            contentDisposition.SetHttpFileName(filename);

            var httpResponse = request.CreateResponse(HttpStatusCode.OK);
            httpResponse.Headers.Add("Content-Type", contentType);
            httpResponse.Headers.Add("Content-Length", content.Length.ToString());
            httpResponse.Headers.Add(HeaderNames.ContentDisposition, contentDisposition.ToString());
            httpResponse.WriteBytes(content);
            
            return Task.FromResult(httpResponse);
        }

        public static Task<HttpResponseData> CreateObjectCreatedResponseAsync(this HttpRequestData request, string location, int id)
        {
            var httpResponse = request.CreateResponse(HttpStatusCode.Created);
            var uriBuilder = new UriBuilder(request.Url.Scheme, request.Url.Host)
            {
                Path = $"api/{location}/{id}",
            };

            if(!request.Url.IsDefaultPort)
                uriBuilder.Port = request.Url.Port;
            
            httpResponse.Headers.Add("Location", uriBuilder.ToString());

            return Task.FromResult(httpResponse);
        }

        public static async Task<HttpResponseData> CreateObjectResponseAsync<T>(this HttpRequestData request, T instance)
        {
            var httpResponse = request.CreateResponse(HttpStatusCode.OK);

            await httpResponse.WriteAsJsonAsync<T>(instance);
            
            return httpResponse;
        }
    }
}