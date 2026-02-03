# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files for better layer caching
COPY nhitomi.sln ./
COPY nhitomi/nhitomi.csproj nhitomi/
COPY nhitomi.Core/nhitomi.Core.csproj nhitomi.Core/
COPY nhitomi.Core.UnitTests/nhitomi.Core.UnitTests.csproj nhitomi.Core.UnitTests/

# Restore dependencies
RUN dotnet restore

# Copy everything else
COPY . .

# Build and publish
RUN dotnet publish nhitomi -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
WORKDIR /app

# Install required packages for health checks
RUN apk add --no-cache curl

# Create non-root user for security
RUN adduser -D -u 1000 appuser
USER appuser

# Copy publish output
COPY --from=build --chown=appuser:appuser /app/publish .

# Environment configuration
ENV DOTNET_EnableDiagnostics=0 \
    DOTNET_RUNNING_IN_CONTAINER=true \
    ASPNETCORE_URLS=http://+:8080

# Expose port (non-privileged)
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=10s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Entrypoint
ENTRYPOINT ["dotnet", "nhitomi.dll"]
