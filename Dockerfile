# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files
COPY *.slnx .
COPY Directory.Build.props .
COPY Directory.Build.targets .
COPY global.json .
COPY src/Core/Domain/Tfs.Portfolio.Domain.csproj src/Core/Domain/
COPY src/Core/Application/Tfs.Portfolio.Application.csproj src/Core/Application/
COPY src/Infrastructure/Persistence/Tfs.Portfolio.Infrastructure.Persistence.csproj src/Infrastructure/Persistence/
COPY src/Presentation/WebAPI/Tfs.Portfolio.Api.csproj src/Presentation/WebAPI/
COPY test/UnitTests/Tfs.Portfolio.UnitTests.csproj test/UnitTests/
COPY test/IntegrationTests/Tfs.Portfolio.IntegrationTests.csproj test/IntegrationTests/

# Restore dependencies
RUN dotnet restore Tfs.Portfolio.slnx

# Copy source code
COPY src/Core/Domain/ src/Core/Domain/
COPY src/Core/Application/ src/Core/Application/
COPY src/Infrastructure/Persistence/ src/Infrastructure/Persistence/
COPY src/Presentation/WebAPI/ src/Presentation/WebAPI/

# Build and publish
RUN dotnet publish src/Presentation/WebAPI/Tfs.Portfolio.Api.csproj -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Create non-root user (use useradd which is available in the base image)
RUN useradd --create-home --no-log-init --shell /bin/bash appuser && chown -R appuser:appuser /app
USER appuser

# Copy published output
COPY --from=build /app/publish .

# Expose port
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Entry point
ENTRYPOINT ["dotnet", "Tfs.Portfolio.Api.dll"]