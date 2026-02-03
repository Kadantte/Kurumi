NOTE FOR CLAUDE CODE: Do not make assumptions, mock data, or placeholders. Always implement production-ready code.
GLOBAL UPGRADE REQUIREMENTS
- Upgrade all NuGet packages to the latest stable versions compatible with .NET 8.
- Migrate all projects to .NET 8 (net8.0) and update code for any breaking changes.
- Update the Discord API library to the latest stable version.
- Replace prefix commands with Discord slash commands and update parsing, modules, and documentation accordingly.

# TASK-016 - Deployment and CI/CD Modernization

## Source
- CLAUDE-REPORT section 16 (Deployment and CI/CD)

## Objective
- Update container and CI configuration to modern, supported tooling.

## Tasks
1. Replace deprecated Docker base images with supported .NET SDK/runtime images.
2. Add test execution to CI and ensure failure reporting is explicit.
3. Add build caching to speed up container builds.

## Implementation Notes
- Current Dockerfile uses deprecated `microsoft/dotnet` images.

## Code Snippet
```dockerfile
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish nhitomi -c Release -o /app

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 80
ENTRYPOINT ["dotnet", "nhitomi.dll"]
```

## Acceptance Criteria
- CI builds on supported SDKs and runs tests.
- Docker images build and run without deprecated base images.

