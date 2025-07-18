# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy toàn bộ source code services/ vào /src/services/
COPY services/ ./services/

# Restore
WORKDIR /src/services/Tasco.TaskService/Tasco.TaskService.API
RUN dotnet restore

# Publish
RUN dotnet publish Tasco.TaskService.API.csproj -c Release -o /app/publish

# Final
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Tasco.TaskService.API.dll"]
