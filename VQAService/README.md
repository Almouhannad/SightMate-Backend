# VQAService

## 1. Overview

The `VQAService` is a .NET Core Minimal API application designed for Visual Question Answering (VQA) and Image Captioning (IC). It provides an interface to process images for both VQA and IC, leveraging external AI microservices. It also includes the functionality for managing conversation history.

## 2. Architecture and Main Patterns
Same as used in [OCRService](../OCRService/README.md)

## 3. Folder Structure

The project's folder structure directly reflects the Clean Architecture and its layered design:

*   `VQAService.API/`:
    *   `Program.cs`: Application entry point, service registration (DI), and HTTP request pipeline configuration (Middlewares).
    *   `Dockerfile`: Defines the containerization process for the API.
    *   `appsettings.json`, `appsettings.Development.json`: Application configuration settings.

*   `VQAService.Domain/`:
    *   `Entities/`: Defines the core business entities such as `Conversation`, `HistoryItem`, `ICInput`, `VQAInput`, `ICOutput`, `VQAOutput`, and options like `VQALanguages`, `VQAModels`, `VQAOptions`.
    *   `Interfaces/`: Contains interfaces that define contracts for external dependencies, such as `IConversationsRepository` and `IVQAServiceProvider`.

*   `VQAService.Application/`:
    *   `Health/`: Contains queries and handlers for health checks (`CheckHealthQuery.cs`, `CheckHealthQueryHandler.cs`).
    *   `IC/`: Contains queries, handlers, and validators for Image Captioning processing (`ProcessICQuery.cs`, `ProcessICQueryHandler.cs`, `ProcessICQueryValidator.cs`).
    *   `VQA/`: Contains queries, handlers, and validators for Visual Question Answering processing (`ProcessVQAQuery.cs`, `ProcessVQAQueryHandler.cs`, `ProcessVQAQueryValidator.cs`).
    *   `DependencyInjection.cs`: Configures and registers application-specific services, including Mediator pipeline handlers and decorators.

*   `VQAService.Infrastructure/`:
    *   `Conversations/`:
        *   `ConversationsRepository.cs`: Concrete implementation of `IConversationsRepository`, handling interactions with the MongoDB database for conversation history.
        *   `DAOs/`: Data Access Objects (`ConversationsDAOs.cs`) and Mappers (`ConversationsMappers.cs`) for MongoDB.
    *   `VQA/`:
        *   `VQAServiceProvider.cs`: Concrete implementation of `IVQAServiceProvider`, handling communication with the external VQA microservice.
        *   `DTOs/`: Data Transfer Objects (`VQAServiceDTOs.cs`, `VQAServiceMappers.cs`) used for serialization and deserialization when interacting with the external services.
    *   `DependencyInjection.cs`: Registers infrastructure-specific services, including the `HttpClient`, MongoDB client, and the use of authentication services.

*   `VQAService.Presentation/`:
    *   `Endpoints/`: Defines the API endpoints for different functionalities.
        *   `Health/Check.cs`: Implements the health check API endpoint.
        *   `IC/Process.cs`: Implements the Image Captioning processing API endpoint.
        *   `VQA/Process.cs`: Implements the Visual Question Answering processing API endpoint.
        *   `Tags.cs`: Constants for API endpoint categorization (for API documenting).
    *   `DependencyInjection.cs`: Configures presentation-specific services like exception handlers and registers API endpoints.

*   `VQAService.Config/`:
    *   `Config.cs`: Static class for accessing environment variables related to VQA service and MongoDB configurations.

## 4. Tests Applied in `VQAService.Tests`

The `VQAService.Tests` project contains automated tests to ensure the correctness and reliability of the application. Tests are organized by the architectural layer they target:

*   **Application Tests (`VQAService.Tests/Application/`):**
    *   `IC/ProcessICQueryHandlerTests.cs`: Unit tests for the `ProcessICQueryHandler`, mocking dependencies to test the handler's business logic in isolation.
    *   `IC/ProcessICQueryValidatorTests.cs`: Unit tests for the `ProcessICQueryValidator`, ensuring that validation rules for IC input are correctly applied.
    *   `VQA/ProcessVQAQueryHandlerTests.cs`: Unit tests for the `ProcessVQAQueryHandler`, mocking dependencies to test the handler's business logic in isolation.
    *   `VQA/ProcessVQAQueryValidatorTests.cs`: Unit tests for the `ProcessVQAQueryValidator`, ensuring that validation rules for VQA input are correctly applied.
*   **Infrastructure Tests (`VQAService.Tests/Infrastructure/`):**
    *   `Conversations/ConversationsRepositoryTests.cs`: Integration tests for the `ConversationsRepository`, testing its interaction with a mocked MongoDB database.
    *   `VQA/VQAServiceProviderTests.cs`: Integration tests for the `VQAServiceProvider`, mocking the `HttpMessageHandler` to simulate responses from the external VQA and IC services and verify correct interaction and error handling.

**Testing Frameworks and Tools:**
*   **Xunit:** The primary testing framework.
*   **Moq:** Used for creating mock objects for dependencies, enabling isolated unit testing.

**How to Run Tests:**
1.  Navigate to the root directory of the `VQAService` solution in your terminal.
2.  Execute the following .NET CLI command:
    ```bash
    dotnet test VQAService.Tests
    ```
**Requirements:**
*   **.NET SDK:** You need the .NET9 SDK installed on your machine to build and run the tests.

## 5. How to Configure `.env` File to Work with the Rest of the Backend

The `VQAService` relies on environment variables for configurations related to connecting to the external VQA/IC services and MongoDB. These variables are accessed through the `VQAService.Config` project. Please see [.env.example](./.env.example) for details.

- Variables related to VQA microservice connection must match [VQAService](https://github.com/Almouhannad/SightMate-VQA-Service) project.
- Variables related to mongodb connection must match [docker-compose](../docker-compose.yml) file.