FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["DotnetApi.Tests/DotnetApi.Tests.csproj", "DotnetApi.Tests/"]
COPY ["DotnetApi.Application/DotnetApi.Application.csproj", "DotnetApi.Application/"]
COPY ["DotnetApi.Domains/DotnetApi.Domains.csproj", "DotnetApi.Domains/"]
COPY ["DotnetApi.Infrastructure/DotnetApi.Infrastructure.csproj", "DotnetApi.Infrastructure/"]
COPY ["DotnetApi.WebApi/DotnetApi.WebApi.csproj", "DotnetApi.WebApi/"]
RUN dotnet restore "DotnetApi.Tests/DotnetApi.Tests.csproj"
COPY . .
WORKDIR "/src/DotnetApi.Tests"
RUN dotnet build "./DotnetApi.Tests.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DotnetApi.Tests.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DotnetApi.Tests.dll"]
