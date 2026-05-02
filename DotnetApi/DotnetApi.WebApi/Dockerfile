FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
# Switch to the non-root 'app' user provided by the base image
USER app
WORKDIR /app
EXPOSE 5160
ENV ASPNETCORE_HTTP_PORTS=5160

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["DotnetApi.WebApi/DotnetApi.WebApi.csproj", "DotnetApi.WebApi/"]
COPY ["DotnetApi.Application/DotnetApi.Application.csproj", "DotnetApi.Application/"]
COPY ["DotnetApi.Domains/DotnetApi.Domains.csproj", "DotnetApi.Domains/"]
COPY ["DotnetApi.Infrastructure/DotnetApi.Infrastructure.csproj", "DotnetApi.Infrastructure/"]
RUN dotnet restore "DotnetApi.WebApi/DotnetApi.WebApi.csproj"

# Copy specifically instead of COPY . .
COPY DotnetApi.WebApi/ ./DotnetApi.WebApi/
COPY DotnetApi.Application/ ./DotnetApi.Application/
COPY DotnetApi.Domains/ ./DotnetApi.Domains/
COPY DotnetApi.Infrastructure/ ./DotnetApi.Infrastructure/

WORKDIR "/src/DotnetApi.WebApi"
RUN dotnet build "./DotnetApi.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DotnetApi.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
# Switch to root temporarily to secure files
USER root
WORKDIR /app
COPY --chown=root:root --chmod=555 --from=publish /app/publish .

# Switch back to the non-root app user for execution
USER app
ENTRYPOINT ["dotnet", "DotnetApi.WebApi.dll"]
