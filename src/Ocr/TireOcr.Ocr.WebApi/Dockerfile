FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /
COPY ["src/Ocr/TireOcr.Ocr.WebApi/TireOcr.Ocr.WebApi.csproj", "src/Ocr/TireOcr.Ocr.WebApi/"]
COPY ["src/Ocr/TireOcr.Ocr.Application/TireOcr.Ocr.Application.csproj", "src/Ocr/TireOcr.Ocr.Application/"]
COPY ["src/Ocr/TireOcr.Ocr.Domain/TireOcr.Ocr.Domain.csproj", "src/Ocr/TireOcr.Ocr.Domain/"]
COPY ["src/Ocr/TireOcr.Ocr.Infrastructure/TireOcr.Ocr.Infrastructure.csproj", "src/Ocr/TireOcr.Ocr.Infrastructure/"]
COPY ["src/TireOcr.ServiceDefaults/TireOcr.ServiceDefaults.csproj", "src/TireOcr.ServiceDefaults/"]
COPY ["src/TireOcr.Shared/TireOcr.Shared.csproj", "src/TireOcr.Shared/"]
RUN dotnet restore "src/Ocr/TireOcr.Ocr.WebApi/TireOcr.Ocr.WebApi.csproj"
COPY . .
WORKDIR "/src/Ocr/TireOcr.Ocr.WebApi"
RUN dotnet build "TireOcr.Ocr.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TireOcr.Ocr.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080
ENV DOTNET_VERSION=8.0.16 ASPNET_VERSION=8.0.15 APP_UID=1654 ASPNETCORE_HTTP_PORTS=8080 DOTNET_RUNNING_IN_CONTAINER=true
ENTRYPOINT ["dotnet", "TireOcr.Ocr.WebApi.dll"]
