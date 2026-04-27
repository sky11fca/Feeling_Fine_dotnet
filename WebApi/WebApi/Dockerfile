FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
ARG API_URL=http://localhost:5160
ARG AI_URL=http://localhost:8000
WORKDIR /src

# Copy specifically
COPY ["WebApi/WebApi.csproj", "WebApi/"]
RUN dotnet restore "WebApi/WebApi.csproj"

COPY WebApi/ ./WebApi/
WORKDIR "/src/WebApi"

# Inject URLs into appsettings.json
RUN sed -i "s|API_URL_PLACEHOLDER|$API_URL|g" wwwroot/appsettings.json
RUN sed -i "s|AI_URL_PLACEHOLDER|$AI_URL|g" wwwroot/appsettings.json

RUN dotnet build "./WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM nginxinc/nginx-unprivileged:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot .
COPY WebApi/nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 8080
