 # Clean Architecture Function App 5.0
<br/>

This is a solution which uses the Clean Architecture templates to demonstrate how to integrate with Azure Functions using .NET 5. This is work in progress as the Azure Functions Worker is in preview.

## Technologies

* Azure Functions Worker
* Entity Framework Core 5
* MediatR
* AutoMapper
* FluentValidation
* NUnit, FluentAssertions, Moq & Respawn
* Docker

## Getting Started

1. Install the latest [.NET 5 SDK](https://dotnet.microsoft.com/download/dotnet/5.0)
2. Install the latest [Azure Function Core Tools](https://docs.microsoft.com/en-us/azure/azure-functions/functions-run-local?tabs=windows%2Ccsharp%2Cbash#install-the-azure-functions-core-tools)
3. Open the solution in Visual Studio or Visual Studio Code (Preferred)
4. Press F5 and the function app should start
5. For Visual Studio Code, Locate **FunctionApp/TodosTesting.http** file and execute API calls with REST Client extension

Check out Jason Taylor's [blog post](https://jasontaylor.dev/clean-architecture-getting-started/) for more information on getting started
with clean architecture and using the original template.

### Docker Configuration
TBA

### Database Configuration

The template is configured to use an in-memory database by default. This ensures that all users will be able to run the solution without needing to set up additional infrastructure (e.g. SQL Server).

If you would like to use SQL Server, you will need to update **FunctionApp/appsettings.json** as follows:

```json
  "UseInMemoryDatabase": false,
```

Verify that the **DefaultConnection** connection string within **appsettings.json** points to a valid SQL Server instance. 

When you run the application the database will be automatically created (if necessary) and the latest migrations will be applied.

### Database Migrations

To use `dotnet-ef` for your migrations please add the following flags to your command (values assume you are executing from repository root)

* `--project src/Infrastructure` (optional if in this folder)
* `--startup-project src/FunctionApp`
* `--output-dir Persistence/Migrations`

For example, to add a new migration from the root folder:

 `dotnet ef migrations add "SampleMigration" --project src\Infrastructure --startup-project src\FunctionApp --output-dir Persistence\Migrations`

## Overview

### Domain

This will contain all entities, enums, exceptions, interfaces, types and logic specific to the domain layer.

### Application

This layer contains all application logic. It is dependent on the domain layer, but has no dependencies on any other layer or project. This layer defines interfaces that are implemented by outside layers. For example, if the application need to access a notification service, a new interface would be added to application and an implementation would be created within infrastructure.

### Infrastructure

This layer contains classes for accessing external resources such as file systems, web services, smtp, and so on. These classes should be based on interfaces defined within the application layer.

### FunctionApp

This layer is the entrance to the application and utilises the Azure Functions runtime. This layer is designed to be thin veneer within minimal logic and no application concerns. It depends on both the Application and Infrastructure layers, however, the dependency on Infrastructure is only to support dependency injection. Therefore only *Startup.cs* should reference Infrastructure.

## Support

If you are having problems, please let us know by [raising a new issue](https://github.com/swilkodev/CleanFunc50/issues/new/choose).

## License

This project is licensed with the [MIT license](LICENSE).
