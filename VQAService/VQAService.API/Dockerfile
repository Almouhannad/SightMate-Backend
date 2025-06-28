# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["VQAService/VQAService.API/VQAService.API.csproj", "VQAService/VQAService.API/"]
COPY ["Shared/SharedKernel/SharedKernel.csproj", "Shared/SharedKernel/"]
COPY ["VQAService/VQAService.Application/VQAService.Application.csproj", "VQAService/VQAService.Application/"]
COPY ["VQAService/VQAService.Config/VQAService.Config.csproj", "VQAService/VQAService.Config/"]
COPY ["VQAService/VQAService.Domain/VQAService.Domain.csproj", "VQAService/VQAService.Domain/"]
COPY ["VQAService/VQAService.Infrastructure/VQAService.Infrastructure.csproj", "VQAService/VQAService.Infrastructure/"]
COPY ["VQAService/VQAService.Presentation/VQAService.Presentation.csproj", "VQAService/VQAService.Presentation/"]
RUN dotnet restore "./VQAService/VQAService.API/VQAService.API.csproj"
COPY . .
WORKDIR "/src/VQAService/VQAService.API"
RUN dotnet build "./VQAService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./VQAService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VQAService.API.dll"]