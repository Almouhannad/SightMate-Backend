# OCRService

## 1. Overview

The `OCRService` is a .NET Core Minimal API application designed for Optical Character Recognition (OCR). It provides an interface to process images and extract text, leveraging external OCR service providers from OCR AI Microservice.

## 2. Architecture and Main Patterns

This project adheres to **Clean Architecture** principles, promoting a clear separation of concerns, testability, and maintainability. It is structured into distinct layers:

*   **Domain:** Contains core business entities, value objects, and interfaces that define the business rules and contracts. This layer is independent of any external frameworks or technologies.
*   **Application:** Orchestrates the domain entities to fulfill use cases. It contains application-specific business rules, queries, commands, and their handlers.
*   **Infrastructure:** Provides concrete implementations of interfaces defined in the Domain layer, handling external concerns such as data access, and external service integrations.
*   **Presentation:** Defines the API endpoints, handles HTTP requests, and maps them to the appropriate application layer commands or queries.
*   **API:** The entry point of the application, responsible for bootstrapping the application, configuring the web host, and registering services from other layers (Using Dependency Injection techniques).
*   **Config:** Centralized project for managing application configurations, reading from environment variables.

The project heavily utilizes the **Mediator pattern**. This pattern facilitates communication between components (e.g., Presentation layer and Application layer) without direct coupling. All requests (queries and commands) flow through a pipeline that incorporates cross-cutting concerns:

*   **Validation Pipeline:** Uses `FluentValidation` to validate incoming requests (queries/commands) before they are processed by their respective handlers, ensuring data integrity and correctness.
*   **Logging Pipeline:** Integrates `Serilog` for structured logging, to log the requests details before and after the're handled

## 3. Folder Structure

The project's folder structure directly reflects the Clean Architecture and its layered design:

*   `OCRService.API/`:
    *   `Program.cs`: Application entry point, service registration (DI), and HTTP request pipeline configuration (Middlewares).
    *   `Dockerfile`: Defines the containerization process for the API.
    *   `appsettings.json`, `appsettings.Development.json`: Application configuration settings.

*   `OCRService.Domain/`:
    *   `Entities/`: Defines the core business entities such as `OCRInput`, `OCROutput`, `OCROutputItem`, `Rectangle`, and options like `OCRLanguages`, `OCRModels`, `OCROptions`.
    *   `Interfaces/`: Contains interfaces that define contracts for external dependencies, such as `IOCRServiceProvider`.    

*   `OCRService.Application/`:
    *   `Health/`: Contains queries and handlers for health checks (e.g., `CheckHealthQuery.cs`, `CheckHealthQueryHandler.cs`).
    *   `OCR/`: Contains queries, handlers, and validators for OCR processing (e.g., `ProcessOCRQuery.cs`, `ProcessOCRQueryHandler.cs`, `ProcessOCRQueryValidator.cs`).
    *   `DependencyInjection.cs`: Configures and registers application-specific services, including Mediator pipeline handlers and decorators.

*   `OCRService.Infrastructure/`:
    *   `OCR/`:
        *   `OCRServiceProvider.cs`: Concrete implementation of `IOCRServiceProvider`, handling communication with the external OCR service.
        *   `DTOs/`: Data Transfer Objects (`OCRInputDTO.cs`, `OCROutputDTO.cs`) used for serialization and deserialization when interacting with the external OCR service.    
    *   `DependencyInjection.cs`: Registers infrastructure-specific services, including the `HttpClient` and the `OCRServiceProvider`.        

*   `OCRService.Presentation/`:
    *   `Endpoints/`: Defines the API endpoints for different functionalities.
        *   `Health/Check.cs`: Implements the health check API endpoint.
        *   `OCR/Process.cs`: Implements the OCR processing API endpoint.
        *   `Tags.cs`: Constants for API endpoint categorization (for API documenting).
    *   `DependencyInjection.cs`: Configures presentation-specific services like exception handlers and registers API endpoints.        

*   `OCRService.Config/`:
    *   `Config.cs`: Static class for accessing environment variables related to OCR service configuration.


## 4. Tests Applied in `OCRService.Tests`

The `OCRService.Tests` project contains automated tests to ensure the correctness and reliability of the application. Tests are organized by the architectural layer they target:

*   **Application Tests (`OCRService.Tests/Application/OCR/`):**
    *   `ProcessOCRQueryHandlerTests.cs`: Unit tests for the `ProcessOCRQueryHandler`, mocking dependencies (`IOCRServiceProvider`) to test the handler's business logic in isolation.
    *   `ProcessOCRQueryValidatorTests.cs`: Unit tests for the `ProcessOCRQueryValidator`, ensuring that validation rules for OCR input are correctly applied.
*   **Domain Tests (`OCRService.Tests/Domain/Entities/Input/`):**
    *   `OCRInputTests.cs`: Unit tests for the `OCRInput` entity, verifying its creation logic and associated business rules.
*   **Infrastructure Tests (`OCRService.Tests/Infrastructure/OCR/`):**
    *   `OCRServiceProviderTests.cs`: Integration tests for the `OCRServiceProvider`, mocking the `HttpMessageHandler` to simulate responses from the external OCR service and verify correct interaction and error handling.

**Testing Frameworks and Tools:**
*   **Xunit:** The primary testing framework.
*   **Moq:** Used for creating mock objects for dependencies, enabling isolated unit testing.

**How to Run Tests:**
1.  Navigate to the root directory of the `OCRService` solution in your terminal.
2.  Execute the following .NET CLI command:
    ```bash
    dotnet test OCRService.Tests
    ```
**Requirements:**
*   **.NET SDK:** You need the .NET9 SDK installed on your machine to build and run the tests.    

## 5. How to Configure `.env` File to Work with the Rest of the Backend

The `OCRService` relies on environment variables for configurations of connecting to the external OCR service. These variables are accessed through the `OCRService.Config` project. Please see [.env.examples](./.env.example) for details.

- OCR Service api variables must match the [SightMate.OCR_Service](../../SightMate.OCR_Service/) project