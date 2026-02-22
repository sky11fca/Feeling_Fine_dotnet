FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
ENV ASPNETCORE_ENVIRONMENT=Development

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["DotnetApi.WebApi/DotnetApi.WebApi.csproj", "DotnetApi.WebApi/"]
COPY ["DotnetApi.Application/DotnetApi.Application.csproj", "DotnetApi.Application/"]
COPY ["DotnetApi.Infrastructure/DotnetApi.Infrastructure.csproj", "DotnetApi.Infrastructure/"]
COPY ["DotnetApi.Domains/DotnetApi.Domains.csproj", "DotnetApi.Domains/"]
RUN dotnet restore "./DotnetApi.WebApi/DotnetApi.WebApi.csproj"
COPY . .
WORKDIR "/src/DotnetApi.WebApi"
RUN dotnet build "./DotnetApi.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DotnetApi.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DotnetApi.WebApi.dll"]