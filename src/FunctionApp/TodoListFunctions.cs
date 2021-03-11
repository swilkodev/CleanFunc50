using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitecture.Application.TodoLists.Commands.CreateTodoList;
using CleanArchitecture.Application.TodoLists.Commands.DeleteTodoList;
using CleanArchitecture.Application.TodoLists.Commands.UpdateTodoList;
using CleanArchitecture.Application.TodoLists.Queries.ExportTodos;
using CleanArchitecture.Application.TodoLists.Queries.GetTodos;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.FunctionApp
{
    public class TodoListFunctions
    {
        private readonly IHttpRequestProcessor processor;
        private readonly ILogger<TodoListFunctions> logger;

        public TodoListFunctions(IHttpRequestProcessor processor, ILogger<TodoListFunctions> logger)
        {
            this.logger = logger;
            this.processor = processor;
        }

        [Function(nameof(GetTodos))]
        public async Task<HttpResponseData> GetTodos([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todolists")]
            HttpRequestData req,
            FunctionContext functionContext)
        {
            logger.LogInformation("Called GetTodos");

            return await this.processor.ExecuteAsync<GetTodosQuery, TodosVm>(functionContext,
                                                                req,
                                                                new GetTodosQuery(),
                                                                (r) => req.CreateObjectResponseAsync(r));
        }

        [Function(nameof(CreateTodosList))]
        public async Task<HttpResponseData> CreateTodosList([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "todolists")]
            TodoList todoList,
            HttpRequestData req,
            FunctionContext functionContext)
        {
            logger.LogInformation("Called CreateTodosList");

            var command = new CreateTodoListCommand
            {
                // TODO Complex types are not set properly by the bindings yet
                Title = "test123"//todoList.Title
            };

            return await this.processor.ExecuteAsync<CreateTodoListCommand, int>(functionContext,
                                                                req,
                                                                command,
                                                                (r) => req.CreateCreatedObjectResponseAsync("todolists", r));
        }

        [Function(nameof(UpdateTodosList))]
        public async Task<HttpResponseData> UpdateTodosList([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "todolists/{id}")] TodoList todoList,
            HttpRequestData req,
            int id,
            FunctionContext functionContext)
        {
            logger.LogInformation("Called UpdateTodosList");

            var request = new UpdateTodoListCommand
            {
                Id = id,
                Title = todoList.Title
            };

            return await this.processor.ExecuteAsync<UpdateTodoListCommand, Unit>(functionContext,
                                                                req,
                                                                request,
                                                                (r) => req.CreateResponseAsync());
        }

        [Function(nameof(DeleteTodosList))]
        public async Task<HttpResponseData> DeleteTodosList([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "todolists/{id}")]
            HttpRequestData req,
            int id,
            FunctionContext functionContext)
        {
            logger.LogInformation("Called DeleteTodosList");

            var request = new DeleteTodoListCommand
            {
                Id = id
            };

            return await this.processor.ExecuteAsync<DeleteTodoListCommand, Unit>(functionContext,
                                                                req,
                                                                request,
                                                                (r) => req.CreateResponseAsync(System.Net.HttpStatusCode.NoContent));
        }
    }

    public class TodoList
    {
        public string Title { get; set; }
    }
}