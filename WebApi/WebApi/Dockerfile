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
# Switch to root temporarily to copy files with root ownership securely
USER root

WORKDIR /usr/share/nginx/html
# Copy web assets and configure them as read-only, owned by root
COPY --chown=root:root --chmod=555 --from=publish /app/publish/wwwroot .

# Copy custom nginx configuration securely
COPY --chown=root:root --chmod=555 WebApi/nginx.conf /etc/nginx/nginx.conf

# Switch back to the unprivileged nginx user for secure execution
USER nginx

EXPOSE 8080
