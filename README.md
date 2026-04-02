# GreekGame

A .NET 10.0 city-building game API built with ASP.NET Core.

## Features

- Create and manage cities
- Build and upgrade buildings (Farms and Houses)
- Resource management (Population, Food, Money)
- RESTful API endpoints

## Getting Started

### Prerequisites

- .NET 10.0 SDK
- Docker (optional, for containerized deployment)

### Running Locally

1. Clone the repository
2. Restore dependencies:
   ```bash
   dotnet restore
   ```
3. Run the application:
   ```bash
   dotnet run --project GreekGame.API
   ```

### Running Tests

```bash
dotnet test
```

### Building with Docker

```bash
docker build -t greengame-api ./GreekGame.API
docker run -p 8080:8080 greengame-api
```

## CI/CD Pipeline

This project uses GitHub Actions for continuous integration. The CI pipeline:

- Runs on every push and pull request to `main`/`master` branches
- Checks code formatting with `dotnet format`
- Builds the .NET application in Release configuration
- Runs StyleCop linting with warnings as errors
- Runs unit tests with code coverage reporting
- Generates HTML and text coverage reports
- Builds and tests the Docker image

### Pipeline Status

Check the Actions tab in GitHub to see the current status of the CI pipeline.

### Code Quality Tools

- **StyleCop.Analyzers**: Enforces C# coding standards and style guidelines
- **EditorConfig**: Ensures consistent code formatting across different editors
- **Code Coverage**: Reports test coverage using coverlet and ReportGenerator

### Code Coverage

The project generates code coverage reports that show:
- Line coverage percentage
- Branch coverage percentage
- Method coverage percentage

Coverage reports are available as artifacts in the GitHub Actions workflow runs.

## API Endpoints

- `POST /api/cities` - Create a new city
- `GET /api/cities/{id}` - Get city details
- `POST /api/cities/{id}/build` - Build a building in a city
- `POST /api/cities/buildings/{id}/upgrade` - Upgrade a building

## Project Structure

- `GreekGame.API/` - Main ASP.NET Core API project
- `GreekGame.Tests/` - Unit tests
- `.github/workflows/` - GitHub Actions CI pipeline
