FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "Tasco.Orchestrator.Api/Tasco.Orchestrator.Api.csproj"
RUN dotnet publish "Tasco.Orchestrator.Api/Tasco.Orchestrator.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Tasco.Orchestrator.Api.dll"] 