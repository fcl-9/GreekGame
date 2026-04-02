# GreekGame

A .NET 10.0 city-building game API built with ASP.NET Core with comprehensive game simulation, resource management, and economic systems.

## Features

- Create and manage cities with dynamic population growth
- Build and upgrade buildings (Farms and Houses)
- Advanced resource management (Population, Food, Money)
- Background game loop service for continuous simulation
- RESTful API endpoints with full CRUD operations
- Market-based economic simulation with supply/demand pricing
- Random event generation system
- Comprehensive test suite (69+ unit tests)
- Docker containerization support
- Automated CI/CD pipeline with GitHub Actions

## Getting Started

### Prerequisites

- .NET 10.0 SDK
- Git
- Docker (optional, for containerized deployment)

### Running Locally

1. Clone the repository
   ```bash
   git clone <repository-url>
   cd GreekGame
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

3. Run the application:
   ```bash
   dotnet run --project GreekGame.API
   ```

4. Access the API at `http://localhost:5000`

### Running Tests

Run all tests:
```bash
dotnet test
```

Run specific test class:
```bash
dotnet test --filter "GreekGame.Tests.CityControllerTests"
```

Run with code coverage:
```bash
dotnet test /p:CollectCoverage=true /p:CoverageFormat=opencover
```

### Building with Docker

```bash
docker build -t greekgame-api ./GreekGame.API
docker run -p 8080:8080 greekgame-api
```

## Testing

The project includes a comprehensive test suite with **69+ unit tests** covering:

### Test Coverage

| Component | Tests | Coverage |
|-----------|-------|----------|
| CityController | 34 | API endpoints and request handling |
| CityService | 13 | Business logic and data operations |
| GameLoopService | 21 | Simulation and game mechanics |
| BuildingUpgrades | 9 | Building cost calculations |
| **Total** | **69** | **All major features** |

### Test Categories

- **Unit Tests**: Fast, isolated component testing with mocks
- **Integration Tests**: Database and multi-component interactions
- **Simulation Tests**: Game loop and economic mechanics

### Running Specific Tests

```bash
# City Controller Tests
dotnet test --filter "CityControllerTests"

# Game Loop Service Tests
dotnet test --filter "GameLoopServiceTests"

# City Service Tests
dotnet test --filter "CityServiceTests"

# Building Upgrades Tests
dotnet test --filter "BuildingUpgradesTests"
```

## Game Mechanics

### Food Production & Consumption

- **Farms** produce 5 food per level per tick
- **Population** consumes 1 food per tick
- Starvation reduces population by 1 per tick

### Population Growth

- Population increases by 1 if: `Food >= Population AND Population < MaxPopulation`
- Population decreases by 1 if: `Food < Population`
- Maximum population determined by housing: `5 × House Level`

### Economy

- Each citizen generates 0.5 money per tick
- Money can be used to build and upgrade buildings

### Market System

- Prices adjust based on supply and demand ratio
- Formula: `NewPrice = OldPrice × (TotalDemand / TotalSupply)`
- Price clamped between 1 and 100

### Random Events

- 10% chance per tick to trigger a random event
- Event types: Drought, Boom, Tax Penalty, None
- Events last 3 ticks

## CI/CD Pipeline

This project uses **GitHub Actions** for continuous integration and deployment.

### Workflows

#### 1. CI Pipeline (`ci.yml`)

Runs on every push and pull request to `main`, `master`, or `develop` branches:

- ✅ **Code Formatting Check**: Verifies C# code style compliance
- ✅ **Build**: Compiles in Release configuration
- ✅ **Testing**: Runs all 69+ unit tests
- ✅ **Code Coverage**: Generates coverage reports
- ✅ **Security Scanning**: Trivy vulnerability scanner
- ✅ **Docker Build**: Builds and validates Docker image

#### 2. Publish Pipeline (`publish.yml`)

Runs on push to `main`/`master` and version tags:

- 🐳 **Docker Registry**: Pushes to GitHub Container Registry
- 📦 **Smart Tagging**: Semantic versioning support
- 📄 **NuGet Publishing**: Optional package publishing on tags

### Pipeline Status

Check the **Actions** tab in GitHub to see the current status of all workflows.

### Code Quality Tools

| Tool | Purpose |
|------|---------|
| **dotnet format** | Code formatting and style validation |
| **Code Coverage** | Test coverage reporting (Coverlet + ReportGenerator) |
| **Trivy** | Container vulnerability scanning |
| **xUnit** | Unit testing framework |
| **Moq** | Mocking and test isolation |

### Code Coverage Reports

Coverage artifacts include:
- HTML reports with detailed breakdowns
- Text summary reports
- SARIF format for GitHub integration

Coverage reports are available as artifacts in GitHub Actions workflow runs.

## API Endpoints

### Cities

```
POST   /api/cities                      # Create a new city
GET    /api/cities/{id}                 # Get city details
GET    /api/cities/{id}/events          # Get active city events
```

### Buildings

```
POST   /api/cities/{id}/build           # Build a new building
POST   /api/cities/buildings/{id}/upgrade # Upgrade an existing building
```

## Project Structure

```
GreekGame/
├── GreekGame.API/                  # Main ASP.NET Core API
│   ├── API/
│   │   └── Controllers/           # API endpoints
│   ├── Application/               # Business logic services
│   ├── BackgroundServices/        # Game loop service
│   ├── Domain/                    # Entity models
│   ├── Infrastructure/            # Database context
│   └── Program.cs                # Startup configuration
├── GreekGame.Tests/              # Unit tests
│   ├── CityControllerTests.cs
│   ├── CityServiceTests.cs
│   ├── GameLoopServiceTests.cs
│   └── BuildingUpgradesTests.cs
├── .github/
│   ├── workflows/               # GitHub Actions workflows
│   │   ├── ci.yml              # CI pipeline
│   │   └── publish.yml         # Publish pipeline
│   └── CI-CD-DOCUMENTATION.md  # Detailed pipeline docs
├── README.md                    # This file
└── *.md                        # Additional documentation
```

## Documentation

- **CI-CD-DOCUMENTATION.md** - Detailed CI/CD pipeline documentation
- **UNIT_TESTS_SUMMARY.md** - Unit test suite overview
- **GAMELOOP_TESTS_SUMMARY.md** - Game loop and simulation tests

## Technologies Used

- **Framework**: .NET 10.0, ASP.NET Core
- **Testing**: xUnit, Moq
- **Database**: Entity Framework Core with SQLite (in-memory for tests)
- **CI/CD**: GitHub Actions
- **Containerization**: Docker
- **Monitoring**: Code Coverage (Coverlet + ReportGenerator)

## Development

### Code Style

- C# coding standards via StyleCop.Analyzers
- EditorConfig for cross-editor consistency
- Nullable reference types enabled

### Testing Strategy

- **AAA Pattern**: Arrange-Act-Assert structure
- **Service Mocking**: Isolated unit tests with Moq
- **In-Memory Database**: Integration tests with EF Core
- **Reflection Invocation**: Testing private methods

## Building and Deployment

### Local Development

```bash
# Build the solution
dotnet build

# Run in debug mode
dotnet run --project GreekGame.API

# Run tests with watch mode
dotnet watch test
```

### Production Deployment

The project supports containerized deployment via Docker:

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10.0
COPY --from=builder /app .
ENTRYPOINT ["dotnet", "GreekGame.API.dll"]
```

## Performance

- Game loop runs every 5 seconds
- 1 tick per 5 seconds of real time
- Optimized database queries with Entity Framework Core
- In-memory caching for city state

## Contributing

1. Create a feature branch
2. Make your changes
3. Run tests: `dotnet test`
4. Push to GitHub (CI pipeline runs automatically)
5. Create a pull request

## License

This project is licensed under the MIT License.

## Support

For issues or questions:
1. Check existing GitHub Issues
2. Review the documentation files
3. Check CI/CD logs in GitHub Actions
