# GEMINI.md

## Project Overview
FeelingFine is a comprehensive full-stack platform designed for managing local businesses, client reviews, and ratings. It integrates a .NET-based microservices architecture with a Python-powered AI module for sentiment analysis and automated response generation.

### Core Components
- **DotnetApi:** A .NET 10 Web API following Clean Architecture and CQRS patterns. It handles business logic, persistence (EF Core with PostgreSQL/Supabase), and authentication.
- **PythonAI:** A FastAPI service that provides AI capabilities, including satisfaction analysis and reply generation, utilizing Ollama (Gemma:2b model) and Hugging Face transformers.
- **WebApi:** A modern, interactive frontend built with Blazor WebAssembly, utilizing MudBlazor for UI components and following a service-based architecture for API communication.
- **Infrastructure:** Fully containerized environment using Docker Compose, including local LLM hosting via Ollama.

## Technical Stack
- **Backend (.NET):** C# 10, EF Core, MediatR (CQRS), FluentValidation, Npgsql, Swagger/OpenAPI.
- **AI Service (Python):** FastAPI, Pydantic, Ollama SDK, Transformers (Hugging Face), Pytest.
- **Frontend (Web):** Blazor WASM, MudBlazor, .NET 10.
- **Database:** PostgreSQL (via Supabase or local container).
- **DevOps:** Docker, Docker Compose, GitHub Actions.

## Building and Running

### Prerequisites
- Docker and Docker Compose
- .NET 10 SDK (for local development)
- Python 3.10+ (for local AI development)

### Using Docker Compose (Recommended)
To start the entire ecosystem including the local LLM:
```bash
docker-compose up --build
```
This will start:
- `ollama`: Local LLM engine.
- `ollama-init`: Helper to pull the `gemma:2b` model.
- `python-ai`: FastAPI service on port 8000.
- `dotnet-api`: .NET API on port 5160.
- `web-ui`: Blazor WASM app on port 3000.

### Local Development
- **DotnetApi:**
  ```bash
  cd DotnetApi
  dotnet run --project DotnetApi.WebApi
  ```
- **PythonAI:**
  ```bash
  cd PythonAI
  pip install -r requirements.txt
  uvicorn main:app --reload
  ```
- **WebApi:**
  ```bash
  cd WebApi/WebApi
  dotnet run
  ```

## Testing

### .NET Tests
Executed using xUnit and FluentAssertions.
```bash
cd DotnetApi
dotnet test
```

### Python AI Tests
Executed using Pytest with coverage reporting.
```bash
cd PythonAI
pytest
```

## Development Conventions

### Architecture & Patterns
- **Clean Architecture:** Strict separation between `Domain`, `Application`, `Infrastructure`, and `WebApi` layers in the .NET project.
- **CQRS:** All business operations in the .NET API are implemented as MediatR Commands or Queries.
- **Validation:** Use FluentValidation for request validation before hitting the handlers.
- **Repository Pattern:** Abstract data access through interfaces in the `Application` layer, implemented in `Infrastructure`.
- **FastAPI Modularization:** Routers are separated into the `api/` directory, services in `service/`, and models in `model/`.

### Coding Standards
- **.NET:** Follow standard C# naming conventions and SOLID principles. Use file-scoped namespaces.
- **Python:** Adhere to PEP8 standards. Use Pydantic for data validation and settings management.
- **Naming:** Use descriptive names for Commands (e.g., `AddReviewCommand`) and Queries (e.g., `GetBusinessQuery`).
