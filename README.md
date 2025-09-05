# SightMate Backend
**Please note** that this a README to show overall description about backend, main folders in this project also contain README files with more specific details
## 1. Overview
This project is a microservices-based backend system designed to orchestrate the whole SightMate system by linking between Frontend client and AI microservices in a controlled manner. It leverages Docker for containerization, enabling a consistent and isolated development and deployment environment. This project was built using **ASP.NET9**.

## 2. Components

The project is composed of the following main services and their corresponding top-level folder structure:

1. **OCR Service [/OCRService](./OCRService/)**: Facade for the Optical Character Recognition (OCR) AI Microservice.
1. **VQA Service + VQA Database (Mongo DB) [/VQAService](./VQAService/)**: Facade for the Visual Question Answering (VQA) AI Microservice. The MongoDB database is used to store user conversations related to VQA interactions.
1. **Identity Service + Identity Database (PostgreSQL) [/IdentityService](./IdentityService/)**: Handles user and role management, including authentication and authorization rules. PostgreSQL serves as its persistent data store for users and roles data.
1. **Seq Server**: A centralized logging server for storing and analyzing application logs.
1. **API Gateway [/Gateway](./Gateway/)**: Acts as a **reverse proxy**, directing incoming requests to the appropriate microservice. It also handles auditing by logging every request with user context, performs load balancing across microservices, applies authentication and authorization rules, and enforces rate limiting.
1. **Shared [/SharedKernel](./Shared/SharedKernel/)**: The reusable, shared code used from all services
1. **.docker [/.docker](./.docker)**: Contains Docker-related configuration files, datastores and scripts, such as initialization scripts for databases.

## 3. How to run

### Requirements to run using Docker

To run this project using Docker, you will need the following installed on your system:

1. **Docker Desktop**
1. **Docker Compose**

---

1.  **Environment Variables**: Add a `.env` file for the global configurations. Please see [.env.example](./.env.example) for details.
- Also, a `.env` file must be added in [/OCRService](./OCRService/), [/VQAService](./VQAService/), and [/IdentityService](./IdentityService/) folders. Please see the `README.md` file inside each folder for details  

2.  **Build and Run**: Navigate to the root directory of the project in your terminal and execute the following command:

    ```bash
    docker-compose up --build
    ```
    This command will build the Docker images for all services (if not already built) and then start the containers as defined in `docker-compose.yml` and `docker-compose.override.yml`.

    - Also, you can run it using by opening the [solution_file](./SightMate-Backend.sln) using [VisualStudio](https://visualstudio.microsoft.com/) and choose to run with `docker-compose` as star up project

3.  **Access Services**:
    *   **API Gateway**: Exposed on port `9990` (as defined in `docker-compose.yml`). You can access the API endpoints for other services through this gateway. Please see [Gateway.API.http](./Gateway/Gateway.API/Gateway.API.http) for example.
    *   **Seq Logging Server**: Accessible on port `8081`. You can view logs by navigating to `http://localhost:8081` in your web browser.
