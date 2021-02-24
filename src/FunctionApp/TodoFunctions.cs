using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Application.TodoLists.Commands.CreateTodoList;
using CleanArchitecture.Application.TodoLists.Commands.UpdateTodoList;
using CleanArchitecture.Application.TodoLists.Queries.ExportTodos;
using CleanArchitecture.Application.TodoLists.Queries.GetTodos;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Pipeline;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;

namespace CleanArchitecture.FunctionApp
{
    public class TodoFunctions
    {
        private readonly IHttpRequestProcessor processor;

        public TodoFunctions(IHttpRequestProcessor processor)
        {
            this.processor = processor;
        }

        [FunctionName(nameof(GetTodos))]
        public async Task<TodosVm> GetTodos([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todolists")] 
            HttpRequestData req,
            FunctionExecutionContext executionContext)
        {
            var logger = executionContext.Logger;
            logger.LogInformation("Called GetTodos");

            return await this.processor.ExecuteAsync2<GetTodosQuery, TodosVm>(executionContext, 
                                                                req, 
                                                                new GetTodosQuery());
        }

        [FunctionName(nameof(ExportTodos))]
        public async Task<HttpResponseData> ExportTodos([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todolists/export/{id}")]
            HttpRequestData req,
            int id,
            FunctionExecutionContext executionContext)
        {
            var logger = executionContext.Logger;
            logger.LogInformation("Called ExportTodos");

            var request= new ExportTodosQuery()
            {
                ListId = id
            };
            return await this.processor.ExecuteAsync<ExportTodosQuery, ExportTodosVm>(executionContext, 
                                                                req, 
                                                                request, 
                                                                (r) => HttpFileContentResult(r.Content, r.ContentType, r.FileName));
        }

        [FunctionName(nameof(CreateTodosList))]
        public async Task<HttpResponseData> CreateTodosList([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "todolists")] 
            CreateTodoListCommand request,
            HttpRequestData req,
            FunctionExecutionContext executionContext)
        {
            var logger = executionContext.Logger;
            logger.LogInformation("Called CreateTodosList");

            return await this.processor.ExecuteAsync<CreateTodoListCommand, int>(executionContext, 
                                                                req, 
                                                                request,
                                                                (r) => HttpResult<int>(r));
        }

        [FunctionName(nameof(UpdateTodosList))]
        public async Task UpdateTodosList([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "todolists/{id}")] HttpRequestData req,
            int id,
            FunctionExecutionContext executionContext)
        {
            var logger = executionContext.Logger;
            logger.LogInformation("Called UpdateTodosList");

            var body = req.Body.Value;
            var text = Encoding.UTF8.GetString(body.Span);
            // Hmm ugly!
            dynamic myObject = JsonConvert.DeserializeObject<dynamic>(text);
            var request = new UpdateTodoListCommand
            {
                Id = id,
                Title = myObject.Title
            };

            await this.processor.ExecuteAsync<UpdateTodoListCommand, Unit>(executionContext, 
                                                                req, 
                                                                request);
        }

        private HttpResponseData HttpFileContentResult(byte[] content, string contentType, string filename)
        {
            var contentDisposition = new ContentDispositionHeaderValue("attachment");
            contentDisposition.SetHttpFileName(filename);

            var httpResponse = new HttpResponseData(HttpStatusCode.OK);
            httpResponse.Headers.Add("Content-Type", contentType);
            httpResponse.Headers.Add("Content-Length", content.Length.ToString());
            httpResponse.Headers[HeaderNames.ContentDisposition] = contentDisposition.ToString();
            using (var stream = new MemoryStream(content))
            using (StreamReader reader = new StreamReader(stream))
            {
                httpResponse.Body = reader.ReadToEnd();
            }
            
            
            return httpResponse;
        }

        private HttpResponseData HttpOkResult()
        {
            return new HttpResponseData(HttpStatusCode.OK);
        }

        private HttpResponseData HttpResult<T>(T instance, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            var response = new HttpResponseData(statusCode);

            //response.Headers = headers;
            response.Body = JsonConvert.SerializeObject(instance);

            return response;
        }
    }

    public class TodoList
    {
        public string Title {get;set;}
    }
}