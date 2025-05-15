FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /
COPY ["src/TireOcr.RunnerPrototype/TireOcr.RunnerPrototype.csproj", "src/TireOcr.RunnerPrototype/"]
COPY ["src/TireOcr.ServiceDefaults/TireOcr.ServiceDefaults.csproj", "src/TireOcr.ServiceDefaults/"]
COPY ["src/TireOcr.Shared/TireOcr.Shared.csproj", "src/TireOcr.Shared/"]
RUN dotnet restore "src/TireOcr.RunnerPrototype/TireOcr.RunnerPrototype.csproj"
COPY . .
WORKDIR "/src/TireOcr.RunnerPrototype"
RUN dotnet build "TireOcr.RunnerPrototype.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TireOcr.RunnerPrototype.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080
ENV DOTNET_VERSION=8.0.16 ASPNET_VERSION=8.0.15 APP_UID=1654 ASPNETCORE_HTTP_PORTS=8080 DOTNET_RUNNING_IN_CONTAINER=true
ENTRYPOINT ["dotnet", "TireOcr.RunnerPrototype.dll"]
