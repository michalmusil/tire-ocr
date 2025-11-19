# Bring OpenCVSharp dependencies
FROM michalmusil/dotnet8-noble-opencv as opencv-builder

# BUILD APP
FROM mcr.microsoft.com/dotnet/aspnet:8.0-noble-amd64 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /
COPY ["src/AiPipeline.TireOcr/AiPipeline.TireOcr.Preprocessing/AiPipeline.TireOcr.Preprocessing.WebApi/AiPipeline.TireOcr.Preprocessing.WebApi.csproj", "src/AiPipeline.TireOcr/AiPipeline.TireOcr.Preprocessing/AiPipeline.TireOcr.Preprocessing.WebApi/"]
COPY ["src/AiPipeline.TireOcr/AiPipeline.TireOcr.Preprocessing/AiPipeline.TireOcr.Preprocessing.Application/AiPipeline.TireOcr.Preprocessing.Application.csproj", "src/AiPipeline.TireOcr/AiPipeline.TireOcr.Preprocessing/AiPipeline.TireOcr.Preprocessing.Application/"]
COPY ["src/AiPipeline.TireOcr/AiPipeline.TireOcr.Preprocessing/AiPipeline.TireOcr.Preprocessing.Domain/AiPipeline.TireOcr.Preprocessing.Domain.csproj", "src/AiPipeline.TireOcr/AiPipeline.TireOcr.Preprocessing/AiPipeline.TireOcr.Preprocessing.Domain/"]
COPY ["src/AiPipeline.TireOcr/AiPipeline.TireOcr.Preprocessing/AiPipeline.TireOcr.Preprocessing.Infrastructure/AiPipeline.TireOcr.Preprocessing.Infrastructure.csproj", "src/AiPipeline.TireOcr/AiPipeline.TireOcr.Preprocessing/AiPipeline.TireOcr.Preprocessing.Infrastructure/"]
COPY ["src/AiPipeline.ServiceDefaults/AiPipeline.ServiceDefaults.csproj", "src/AiPipeline.ServiceDefaults/"]
COPY ["src/AiPipeline.Shared/AiPipeline.Shared.csproj", "src/AiPipeline.Shared/"]
RUN dotnet restore "src/AiPipeline.TireOcr/AiPipeline.TireOcr.Preprocessing/AiPipeline.TireOcr.Preprocessing.WebApi/AiPipeline.TireOcr.Preprocessing.WebApi.csproj"
COPY . .
WORKDIR "/src/AiPipeline.TireOcr/AiPipeline.TireOcr.Preprocessing/AiPipeline.TireOcr.Preprocessing.WebApi"
RUN dotnet build "AiPipeline.TireOcr.Preprocessing.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AiPipeline.TireOcr.Preprocessing.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
RUN apt-get update
RUN apt-get install -y libc6 libicu-dev libfontconfig1
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=publish /app/publish/runtimes/linux-x64/native/libSkiaSharp.so /usr/lib/
COPY --from=opencv-builder /usr/lib /usr/lib
# COPY --from=skiasharp-builder out/linux/x64/ /usr/lib

EXPOSE 8080
ENV DOTNET_VERSION=8.0.16 ASPNET_VERSION=8.0.16 APP_UID=1654 ASPNETCORE_HTTP_PORTS=8080 DOTNET_RUNNING_IN_CONTAINER=true
ENTRYPOINT ["dotnet", "AiPipeline.TireOcr.Preprocessing.WebApi.dll"]
