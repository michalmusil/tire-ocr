FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /
COPY ["src/AiPipeline.TireOcr/AiPipeline.TireOcr.Postprocessing/AiPipeline.TireOcr.Postprocessing.WebApi/AiPipeline.TireOcr.Postprocessing.WebApi.csproj", "src/AiPipeline.TireOcr/AiPipeline.TireOcr.Postprocessing/AiPipeline.TireOcr.Postprocessing.WebApi/"]
COPY ["src/AiPipeline.TireOcr/AiPipeline.TireOcr.Postprocessing/AiPipeline.TireOcr.Postprocessing.Application/AiPipeline.TireOcr.Postprocessing.Application.csproj", "src/AiPipeline.TireOcr/AiPipeline.TireOcr.Postprocessing/AiPipeline.TireOcr.Postprocessing.Application/"]
COPY ["src/AiPipeline.TireOcr/AiPipeline.TireOcr.Postprocessing/AiPipeline.TireOcr.Postprocessing.Domain/AiPipeline.TireOcr.Postprocessing.Domain.csproj", "src/AiPipeline.TireOcr/AiPipeline.TireOcr.Postprocessing/AiPipeline.TireOcr.Postprocessing.Domain/"]
COPY ["src/AiPipeline.TireOcr/AiPipeline.TireOcr.Postprocessing/AiPipeline.TireOcr.Postprocessing.Infrastructure/AiPipeline.TireOcr.Postprocessing.Infrastructure.csproj", "src/AiPipeline.TireOcr/AiPipeline.TireOcr.Postprocessing/AiPipeline.TireOcr.Postprocessing.Infrastructure/"]
COPY ["src/AiPipeline.ServiceDefaults/AiPipeline.ServiceDefaults.csproj", "src/AiPipeline.ServiceDefaults/"]
COPY ["src/AiPipeline.Shared/AiPipeline.Shared.csproj", "src/AiPipeline.Shared/"]
RUN dotnet restore "src/AiPipeline.TireOcr/AiPipeline.TireOcr.Postprocessing/AiPipeline.TireOcr.Postprocessing.WebApi/AiPipeline.TireOcr.Postprocessing.WebApi.csproj"
COPY . .
WORKDIR "/src/AiPipeline.TireOcr/AiPipeline.TireOcr.Postprocessing/AiPipeline.TireOcr.Postprocessing.WebApi"
RUN dotnet build "AiPipeline.TireOcr.Postprocessing.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AiPipeline.TireOcr.Postprocessing.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080
ENV DOTNET_VERSION=8.0.16 ASPNET_VERSION=8.0.16 APP_UID=1654 ASPNETCORE_HTTP_PORTS=8080 DOTNET_RUNNING_IN_CONTAINER=true
ENTRYPOINT ["dotnet", "AiPipeline.TireOcr.Postprocessing.WebApi.dll"]