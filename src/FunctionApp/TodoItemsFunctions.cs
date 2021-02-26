using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.TodoItems.Commands.CreateTodoItem;
using CleanArchitecture.Application.TodoItems.Queries.GetTodoItemsWithPagination;
using CleanArchitecture.Application.TodoLists.Queries.ExportTodos;
using CleanArchitecture.Application.TodoLists.Queries.GetTodos;
using CleanArchitecture.FunctionApp;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace FunctionApp
{
    public class TodoItemsFunctions
    {
        private readonly IHttpRequestProcessor processor;
        private readonly ILogger<TodoItemsFunctions> logger;

        public TodoItemsFunctions(IHttpRequestProcessor processor, ILogger<TodoItemsFunctions> logger)
        {
            this.logger = logger;
            this.processor = processor;
        }

        [Function(nameof(GetTodoItems))]
        public async Task<HttpResponseData> GetTodoItems([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todolists/{id}")]
            HttpRequestData req,
            int id,
            FunctionContext functionContext)
        {
            logger.LogInformation("Called GetTodoItems");

            if(req.Headers.TryGetValues("Content-Type", out IEnumerable<string> values))
            {
                if(values.Contains("text/csv"))
                {
                    var request = new ExportTodosQuery()
                    {
                        ListId = id
                    };
                    return await this.processor.ExecuteAsync<ExportTodosQuery, ExportTodosVm>(functionContext,
                                                                        req,
                                                                        request,
                                                                        (r) => req.CreateFileContentResponseAsync(r.Content, r.ContentType, r.FileName));
                }
            }

            var query =  new GetTodoItemsWithPaginationQuery()
            {
                ListId = id
            };

            return await this.processor.ExecuteAsync<GetTodoItemsWithPaginationQuery, PaginatedList<TodoItemDto>>(functionContext,
                                                                req,
                                                                query,
                                                                (r) => req.CreateObjectResponseAsync(r));
        }

        [Function(nameof(CreateTodoItem))]
        public async Task<HttpResponseData> CreateTodoItem([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "todolists/{id}/items")]
            HttpRequestData req,
            TodoList todoList,
            int id,
            FunctionContext functionContext)
        {
            logger.LogInformation("Called CreateTodoItems");

            var command = new CreateTodoItemCommand
            {
                ListId = id,
                // TODO Complex types are not set properly by the bindings yet
                Title = "feed dog"//todoList.Title
            };

            return await this.processor.ExecuteAsync<CreateTodoItemCommand, int>(functionContext,
                                                                req,
                                                                command,
                                                                (r) => req.CreateCreatedObjectResponseAsync($"todolists/{id}/items", r));
        }
    }
}