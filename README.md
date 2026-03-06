# Introducing FeelingFine.net

## Features
- .NET EF Core API: Comments, Ratings, Clients
- Python AI: Satisfaction Analysis, Ratings Analysis, Generating Responses
- Monetization: local businesses
---

## Contents:
- [Web UI](https://github.com/sky11fca/Feeling_Fine_dotnet/#web-ui)
- [Complete Deployment In the Cloud](https://github.com/sky11fca/Feeling_Fine_dotnet/#complete-deployment-in-the-cloud)
- [Docker Images and Containerization](https://github.com/sky11fca/Feeling_Fine_dotnet/#docker-image-and-containerization)
- [Code Quality](https://github.com/sky11fca/Feeling_Fine_dotnet/#code-quality)
- [Tests](https://github.com/sky11fca/Feeling_Fine_dotnet/#tests)
- [SonarQube/Sonarcloud Integration](https://github.com/sky11fca/Feeling_Fine_dotnet/#sonarqubesonarcloud-integration)
- [CI/CD Pipeline](https://github.com/sky11fca/Feeling_Fine_dotnet/#cicd-pipeline)

## Web UI

A modern, interactive, responsive interface using Blazor Web Assembly
This one must consume the .NET API (Endpoints CQRS) and AI module (Django, FastAPI)

Requirements:
- Authentication and Authorization (Identity or OpenID Connect)
- Dashboard Page with graphics and Statistics
- CRUD page for principal entities
- Pages that consumes AI results
- State Managements (Fluxor, Mediatr pattern or Simple State Store)
- Modern UI(MudBlazor)

- The WASM app must be a separate project deployed in Cloud

## Complete Deployment In the cloud

Both the .NET API and Python AI must be containerized and deployed in a cloud service:
- Azure
- AWS
- GCP

Requirements:
- .NET API and AI API in a separated environment
- Frontend Blazor in a separated cloud
- Database in Cloud
- File storage in Cloud

## Docker Images and Containerization

All 3 elements must have
- Optimized Dockerfile
- Build + push in registry (Docker hub)
- Docker-compose for local development
- Kubernetes configuration (Not Necessary)

## Code Quality

Best Practicies:
- Complete CQRS:
  - Separated commands for queries
  - Handlers, Validators, DTO
  - Mediator Pattern
- Clean Architecture:
  - Domain
  - Application
  - Infrastructure
  - API
- SOLID + naming, layering DI
- Python AI: PEP8 + modularization

## Tests
- .NET API
  - With xUnit/NUnit
  - at least 60-70% Coverage
  - Integration tests for REST Endpoints
  - Test Containers
- Python AI:
  - Pytest unit tests
  - Integration tests for Python Endpoints
- E2EE (Not Necessary)
  - API to API Tests

## SonarQube/Sonarcloud Integration

In Pipeline you must include:
- Code Quality analysis
- Vulnerability Detection
- Metrics: Code smells, duplication, coverage
- Quality Gate. For PR acceptance

## CI/CD Pipeline

The Pipeline:
- Build + Automatic Tests
- SonarQube Testing
- Docker Image Building
- Registry Push
- Cloud Deployment

Done via Github Actions/Gitlab CI/Azure Devops
