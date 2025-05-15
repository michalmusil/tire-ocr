FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /
COPY ["src/Postprocessing/TireOcr.Postprocessing.WebApi/TireOcr.Postprocessing.WebApi.csproj", "src/Postprocessing/TireOcr.Postprocessing.WebApi/"]
COPY ["src/Postprocessing/TireOcr.Postprocessing.Application/TireOcr.Postprocessing.Application.csproj", "src/Postprocessing/TireOcr.Postprocessing.Application/"]
COPY ["src/Postprocessing/TireOcr.Postprocessing.Domain/TireOcr.Postprocessing.Domain.csproj", "src/Postprocessing/TireOcr.Postprocessing.Domain/"]
COPY ["src/Postprocessing/TireOcr.Postprocessing.Infrastructure/TireOcr.Postprocessing.Infrastructure.csproj", "src/Postprocessing/TireOcr.Postprocessing.Infrastructure/"]
COPY ["src/TireOcr.ServiceDefaults/TireOcr.ServiceDefaults.csproj", "src/TireOcr.ServiceDefaults/"]
COPY ["src/TireOcr.Shared/TireOcr.Shared.csproj", "src/TireOcr.Shared/"]
RUN dotnet restore "src/Postprocessing/TireOcr.Postprocessing.WebApi/TireOcr.Postprocessing.WebApi.csproj"
COPY . .
WORKDIR "/src/Postprocessing/TireOcr.Postprocessing.WebApi"
RUN dotnet build "TireOcr.Postprocessing.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TireOcr.Postprocessing.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080
ENV DOTNET_VERSION=8.0.16 ASPNET_VERSION=8.0.16 APP_UID=1654 ASPNETCORE_HTTP_PORTS=8080 DOTNET_RUNNING_IN_CONTAINER=true
ENTRYPOINT ["dotnet", "TireOcr.Postprocessing.WebApi.dll"]
