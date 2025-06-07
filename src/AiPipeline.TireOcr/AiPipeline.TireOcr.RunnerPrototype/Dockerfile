FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /
COPY ["src/AiPipeline.TireOcr/AiPipeline.TireOcr.RunnerPrototype/AiPipeline.TireOcr.RunnerPrototype.csproj", "src/AiPipeline.TireOcr/AiPipeline.TireOcr.RunnerPrototype/"]
COPY ["src/AiPipeline.ServiceDefaults/AiPipeline.ServiceDefaults.csproj", "src/AiPipeline.ServiceDefaults/"]
COPY ["src/AiPipeline.Shared/AiPipeline.Shared.csproj", "src/AiPipeline.Shared/"]
RUN dotnet restore "src/AiPipeline.TireOcr/AiPipeline.TireOcr.RunnerPrototype/AiPipeline.TireOcr.RunnerPrototype.csproj"
COPY . .
WORKDIR "/src/AiPipeline.TireOcr/AiPipeline.TireOcr.RunnerPrototype"
RUN dotnet build "AiPipeline.TireOcr.RunnerPrototype.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AiPipeline.TireOcr.RunnerPrototype.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080
ENV DOTNET_VERSION=8.0.16 ASPNET_VERSION=8.0.16 APP_UID=1654 ASPNETCORE_HTTP_PORTS=8080 DOTNET_RUNNING_IN_CONTAINER=true
ENTRYPOINT ["dotnet", "AiPipeline.TireOcr.RunnerPrototype.dll"]
