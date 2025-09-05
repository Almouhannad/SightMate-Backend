# IdentityService

## 1. Overview

The `IdentityService` is a .NET Core Minimal API application responsible for managing user identities, including registration, login, and user profile management. It provides a secure and scalable solution for handling authentication and authorization within the SightMate system.

## 2. Architecture and Main Patterns
Same as used in [OCRService](../OCRService/README.md)

## 3. PostgreSQL Database and ASP.NET Core Identity Framework Usage

The `IdentityService` uses **PostgreSQL** as its relational database for storing user and role data. This is configured within the `IdentityService.Infrastructure` project:

*   **`ApplicationDbContext`**: This class, located at `IdentityService.Infrastructure/Users/ApplicationDbContext.cs`, inherits from `IdentityDbContext<UserDAO, RoleDAO, Guid>`. This integration allows ASP.NET Core Identity to manage user and role data directly within the PostgreSQL database.
*   **Migrations**: Database schema changes are managed through Entity Framework Core Migrations, located in `IdentityService.Infrastructure/Migrations/`. These migrations define how the `UserDAO` and `RoleDAO` tables are created and updated in the database. The `ApplyMigrations()` extension method in `IdentityService.Infrastructure/DependencyInjection.cs` ensures that any pending migrations are applied on application startup in development environments.

**ASP.NET Core Identity** is extensively used for managing user accounts and roles:

*   **`UserDAO` and `RoleDAO`**: These Data Access Objects, found in `IdentityService.Infrastructure/Users/DAOs/`, inherit from `IdentityUser<Guid>` and `IdentityRole<Guid>` respectively. They serve as the concrete implementations of user and role entities that interact with the database via Entity Framework Core.
*   **`UserManagerImplementation`**: This class, located at `IdentityService.Infrastructure/Users/UserManagerImplementation.cs`, implements the `IUserManager` interface (from `IdentityService.Domain/Interfaces/`). It wraps the standard `UserManager<UserDAO>` and `RoleManager<RoleDAO>` from ASP.NET Core Identity, providing an abstraction for user management operations such as creating users, finding users by email/ID, checking passwords, and assigning roles.
*   **Authentication and Authorization**: JWT Bearer authentication is configured in `IdentityService.Infrastructure/DependencyInjection.cs`. This setup validates JWT tokens issued by the `JWTProvider` for authentication. Authorization policies, such as `"AdminOnly"` which checks for the `ADMIN` role, are also defined using ASP.NET Core Identity's authorization features.

## 4. Folder Structure

The project's folder structure directly reflects the Clean Architecture and its layered design:

*   `IdentityService.API/`:
    *   `Program.cs`: Application entry point, service registration (DI), and HTTP request pipeline configuration (Middlewares). Includes setup for Serilog, OpenAPI, and mapping endpoints.
    *   `appsettings.json`, `appsettings.Development.json`: Application configuration settings.
    *   `Dockerfile`: Defines the containerization process for the API.

*   `IdentityService.Domain/`:
    *   `Entities/`: Defines the core business entities such as `User.cs` and `Role.cs`, along with predefined roles in `Roles.cs`.
    *   `Errors/`: Contains custom error definitions related to user operations (`UserErrors.cs`).
    *   `Interfaces/`: Contains interfaces that define contracts for external dependencies, such as `IJWTProvider`, `IUserContext`, and `IUserManager`.    

*   `IdentityService.Application/`:
    *   `Login/`: Contains the `LoginQuery`, `LoginQueryHandler`, `LoginQueryResponse`, and `LoginQueryValidator` for user login functionality.
    *   `Register/`: Contains the `RegisterCommand`, `RegisterCommandHandler`, and `RegisterCommandValidator` for new user registration.
    *   `UserProfile/`: Contains the `GetUserProfileQuery`, `GetUserProfileQueryHandler`, and `GetUserProfileQueryResponse` for fetching user profile details.
    *   `DependencyInjection.cs`: Configures and registers application-specific services, including Mediator handlers, FluentValidation validators, and decorators for validation and logging pipelines.

*   `IdentityService.Infrastructure/`:
    *   `JWT/`: Contains `JWTProvider.cs`, the concrete implementation of `IJWTProvider` for generating JWT tokens.
    *   `Migrations/`: Contains Entity Framework Core migration files for database schema management.
    *   `Users/`:
        *   `ApplicationDbContext.cs`: The Entity Framework Core DbContext integrated with ASP.NET Core Identity.
        *   `DAOs/`: Contains `UserDAO.cs` and `RoleDAO.cs`, the database-mapped representations of users and roles.
        *   `UserContext.cs`: Implementation of `IUserContext` to retrieve user information from the current request context.
        *   `UserManagerImplementation.cs`: Concrete implementation of `IUserManager` utilizing ASP.NET Core Identity's `UserManager` and `RoleManager`.
    *   `DependencyInjection.cs`: Registers infrastructure-specific services, including the `ApplicationDbContext`, ASP.NET Core Identity services, JWT authentication, and authorization policies.

*   `IdentityService.Presentation/`:
    *   `Endpoints/`: Defines the API endpoints for different functionalities.
        *   `Login/LoginUser.cs`: Implements the user login API endpoint.
        *   `Register/RegisterUser.cs`: Implements the user registration API endpoint.
        *   `UserProfile/GetUserProfile.cs`: Implements the user profile retrieval API endpoint.
        *   `Tags.cs`: Constants for API endpoint categorization (for API documenting).
    *   `DependencyInjection.cs`: Configures presentation-specific services like global exception handling and registers API endpoints.

*   `IdentityService.Config/`:
    *   `Config.cs`: Static class for accessing environment variables, specifically the `CONNECTION_STRING` for the database.

## 5. Tests Applied in `IdentityService.Tests`

The `IdentityService.Tests` project contains automated tests to ensure the correctness and reliability of the application. Tests are organized by the architectural layer they target:

*   **Application Tests (`IdentityService.Tests/Application/`):**
    *   `LoginQueryHandlerTests.cs`: Unit tests for the `LoginQueryHandler`, mocking dependencies (`IUserManager`, `IJWTProvider`) to test the handler's business logic in isolation.
    *   `LoginQueryValidatorTests.cs`: Unit tests for the `LoginQueryValidator`, ensuring that validation rules for login input are correctly applied.
    *   Similar tests exist for `RegisterCommandHandler` and `RegisterCommandValidator`.
*   **Infrastructure Tests (`IdentityService.Tests/Infrastructure/`):**
    *   `JWTProviderTests.cs`: Unit tests for the `JWTProvider`, verifying the correct generation of JWT tokens.
    *   `UserManagerImplementationTests.cs`: Integration tests for the `UserManagerImplementation`, mocking the underlying `UserManager` and `RoleManager` to test user and role management operations.

**Testing Frameworks and Tools:**
*   **Xunit:** The primary testing framework.
*   **Moq:** Used for creating mock objects for dependencies, enabling isolated unit testing.

**How to Run Tests:**
1.  Navigate to the root directory of the `IdentityService` solution in your terminal.
2.  Execute the following .NET CLI command:
    ```bash
    dotnet test IdentityService.Tests
    ```
**Requirements:**
*   **.NET SDK:** You need the .NET9 SDK installed on your machine to build and run the tests.

## 6. How to Configure `.env` File to Work with the Rest of the Backend

Please create a `.env` file in this project that contains the connection string for the PostgreSQL database which must match the [docker-compose](../docker-compose.yml) file, please see [.env.example](./.env.example) for details. Also, this project will use JWT options from the `.env` file in the root of the backend project
