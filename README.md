# Clean Architecture Solution Template
![.NET Core](https://github.com/suleymanozev/CleanArchitecture/workflows/.NET/badge.svg)

Fork of [Clean Architecture Solution Template](https://github.com/jasontaylordev/CleanArchitecture)

This is a solution template for creating a ASP.NET Core following the principles of Clean Architecture. Create a new project based on this template by clicking the above **Use this template** button. 

## Technologies

* [ASP.NET Core 6](https://docs.microsoft.com/en-us/aspnet/core/introduction-to-aspnet-core?view=aspnetcore-6.0)
* [Entity Framework Core 6](https://docs.microsoft.com/en-us/ef/core/)
* [PostgreSQL](https://www.postgresql.org/)
* [MediatR](https://github.com/jbogard/MediatR)
* [MassTransit](https://masstransit-project.com/)
* [AutoMapper](https://automapper.org/)
* [FluentValidation](https://fluentvalidation.net/)
* [xUnit](https://xunit.net/), [DotNet.TestContainers](https://github.com/HofmeisterAn/dotnet-testcontainers), [FluentAssertions](https://fluentassertions.com/), [Moq](https://github.com/moq) & [Respawn](https://github.com/jbogard/Respawn)
* [Docker](https://www.docker.com/)

### Docker Configuration

In order to get Docker working, you will need to add a temporary SSL cert and mount a volume to hold that cert.
You can find [Microsoft Docs](https://docs.microsoft.com/en-us/aspnet/core/security/docker-https?view=aspnetcore-6.0) that describe the steps required for Windows, macOS, and Linux.

In order to build and run the docker containers, execute `docker-compose -f 'docker-compose.yml' up --build` from the root of the solution where you find the docker-compose.yml file.  You can also use "Docker Compose" from Visual Studio for Debugging purposes.
Then open http://localhost:5000 on your browser.

To disable Docker in Visual Studio, right-click on the **docker-compose** file in the **Solution Explorer** and select **Unload Project**.

### Certificate Configuration
#### For Windows (CMD):
The following will need to be executed from your terminal to create a cert
`dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\aspnetapp.pfx -p Your_password123`
`dotnet dev-certs https --trust`

#### For Windows (PowerShell):
The following will need to be executed from your terminal to create a cert
`dotnet dev-certs https -ep $env:USERPROFILE\.aspnet\https\aspnetapp.pfx -p Your_password123`
`dotnet dev-certs https --trust`

FOR macOS:
`dotnet dev-certs https -ep ${HOME}/.aspnet/https/aspnetapp.pfx -p Your_password123`
`dotnet dev-certs https --trust`

FOR Linux:
`dotnet dev-certs https -ep ${HOME}/.aspnet/https/aspnetapp.pfx -p Your_password123`

### Database Configuration

Verify that the **DefaultConnection** connection string within **appsettings.json** points to a valid PostgreSQL instance. 

When you run the application the database will be automatically created (if necessary) and the latest migrations will be applied.

### Database Migrations

To use `dotnet-ef` for your migrations please add the following flags to your command (values assume you are executing from repository root)

* `--project src/Infrastructure` (optional if in this folder)
* `--startup-project src/WebUI`
* `--output-dir Persistence/Migrations`

For example, to add a new migration from the root folder:

#### For Windows
`dotnet ef migrations add "SampleMigration" --project src\Infrastructure --startup-project src\WebUI --output-dir Persistence\Migrations`

#### For macOS & Linux
`dotnet ef migrations add "SampleMigration" --project src/Infrastructure --startup-project src/WebUI --output-dir Persistence/Migrations`

## Overview

### Domain

This will contain all entities, enums, exceptions, interfaces, types and logic specific to the domain layer.

### Application

This layer contains all application logic. It is dependent on the domain layer, but has no dependencies on any other layer or project. This layer defines interfaces that are implemented by outside layers. For example, if the application need to access a notification service, a new interface would be added to application and an implementation would be created within infrastructure.

### Infrastructure

This layer contains classes for accessing external resources such as file systems, web services, smtp, and so on. These classes should be based on interfaces defined within the application layer.

### WebUI

This layer is a single page application based on Angular 10 and ASP.NET Core 5. This layer depends on both the Application and Infrastructure layers, however, the dependency on Infrastructure is only to support dependency injection. Therefore only *Startup.cs* should reference Infrastructure.

## Support

If you are having problems, please let us know by [raising a new issue](https://github.com/suleymanozev/CleanArchitecture/issues/new/choose).

## License

This project is licensed with the [MIT license](LICENSE).

Thanks Jason Taylor
[Twitter](https://twitter.com/jasontaylordev), [GitHub](https://github.com/jasontaylordev)
