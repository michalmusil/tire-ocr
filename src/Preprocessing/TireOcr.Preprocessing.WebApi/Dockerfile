# BUILD OPENCVSHARP
FROM mcr.microsoft.com/dotnet/aspnet:8.0-noble-amd64 as opencv-builder
ENV DEBIAN_FRONTEND=noninteractive
ENV OPENCV_VERSION=4.10.0
WORKDIR /
# Install opencv dependencies
RUN apt-get update && apt-get -y install --no-install-recommends \
    apt-transport-https \
    software-properties-common \
    wget \
    unzip \
    ca-certificates \
    build-essential \
    cmake \
    git \
    libtbb-dev \
    libatlas-base-dev \
    libgtk2.0-dev \
    libavcodec-dev \
    libavformat-dev \
    libswscale-dev \
    libdc1394-dev \
    libxine2-dev \
    libv4l-dev \
    libtheora-dev \
    libvorbis-dev \
    libxvidcore-dev \
    libopencore-amrnb-dev \
    libopencore-amrwb-dev \
    x264 \
    libtesseract-dev \
    libgdiplus \
    && apt-get -y clean \
    && rm -rf /var/lib/apt/lists/*

# Setup opencv and opencv-contrib source
RUN wget -q https://github.com/opencv/opencv/archive/${OPENCV_VERSION}.zip && \
    unzip -q ${OPENCV_VERSION}.zip && \
    rm ${OPENCV_VERSION}.zip && \
    mv opencv-${OPENCV_VERSION} opencv && \
    wget -q https://github.com/opencv/opencv_contrib/archive/${OPENCV_VERSION}.zip && \
    unzip -q ${OPENCV_VERSION}.zip && \
    rm ${OPENCV_VERSION}.zip && \
    mv opencv_contrib-${OPENCV_VERSION} opencv_contrib

# Build OpenCV
RUN cd opencv && mkdir build && cd build && \
    cmake \
    -D OPENCV_EXTRA_MODULES_PATH=/opencv_contrib/modules \
    -D CMAKE_BUILD_TYPE=RELEASE \
    -D BUILD_SHARED_LIBS=OFF \
    -D ENABLE_CXX11=ON \
    -D BUILD_EXAMPLES=OFF \
    -D BUILD_DOCS=OFF \
    -D BUILD_PERF_TESTS=OFF \
    -D BUILD_TESTS=OFF \
    -D BUILD_JAVA=OFF \
    -D BUILD_opencv_app=OFF \
    -D BUILD_opencv_barcode=OFF \
    -D BUILD_opencv_java_bindings_generator=OFF \
    -D BUILD_opencv_js_bindings_generator=OFF \
    -D BUILD_opencv_python_bindings_generator=OFF \
    -D BUILD_opencv_python_tests=OFF \
    -D BUILD_opencv_ts=OFF \
    -D BUILD_opencv_js=OFF \
    -D BUILD_opencv_bioinspired=OFF \
    -D BUILD_opencv_ccalib=OFF \
    -D BUILD_opencv_datasets=OFF \
    -D BUILD_opencv_dnn_objdetect=OFF \
    -D BUILD_opencv_dpm=OFF \
    -D BUILD_opencv_fuzzy=OFF \
    -D BUILD_opencv_gapi=OFF \
    -D BUILD_opencv_intensity_transform=OFF \
    -D BUILD_opencv_mcc=OFF \
    -D BUILD_opencv_objc_bindings_generator=OFF \
    -D BUILD_opencv_rapid=OFF \
    -D BUILD_opencv_reg=OFF \
    -D BUILD_opencv_stereo=OFF \
    -D BUILD_opencv_structured_light=OFF \
    -D BUILD_opencv_surface_matching=OFF \
    -D BUILD_opencv_videostab=OFF \
    -D BUILD_opencv_wechat_qrcode=ON \
    -D WITH_GSTREAMER=OFF \
    -D WITH_ADE=OFF \
    -D OPENCV_ENABLE_NONFREE=ON \
    .. && make -j$(nproc) && make install && ldconfig

# Download OpenCvSharp
RUN git clone https://github.com/shimat/opencvsharp.git && cd opencvsharp

# Install the Extern lib.
RUN mkdir /opencvsharp/make && cd /opencvsharp/make && \
    cmake -D CMAKE_INSTALL_PREFIX=/opencvsharp/make /opencvsharp/src && \
    make -j$(nproc) && make install && \
    rm -rf /opencv && \
    rm -rf /opencv_contrib && \
    cp /opencvsharp/make/OpenCvSharpExtern/libOpenCvSharpExtern.so /usr/lib/









# BUILD APP
FROM mcr.microsoft.com/dotnet/aspnet:8.0-noble-amd64 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0-noble-amd64 AS build
WORKDIR /
COPY ["src/Preprocessing/TireOcr.Preprocessing.WebApi/TireOcr.Preprocessing.WebApi.csproj", "src/Preprocessing/TireOcr.Preprocessing.WebApi/"]
COPY ["src/Preprocessing/TireOcr.Preprocessing.Application/TireOcr.Preprocessing.Application.csproj", "src/Preprocessing/TireOcr.Preprocessing.Application/"]
COPY ["src/Preprocessing/TireOcr.Preprocessing.Domain/TireOcr.Preprocessing.Domain.csproj", "src/Preprocessing/TireOcr.Preprocessing.Domain/"]
COPY ["src/Preprocessing/TireOcr.Preprocessing.Infrastructure/TireOcr.Preprocessing.Infrastructure.csproj", "src/Preprocessing/TireOcr.Preprocessing.Infrastructure/"]
COPY ["src/TireOcr.ServiceDefaults/TireOcr.ServiceDefaults.csproj", "src/TireOcr.ServiceDefaults/"]
COPY ["src/TireOcr.Shared/TireOcr.Shared.csproj", "src/TireOcr.Shared/"]
RUN dotnet restore "src/Preprocessing/TireOcr.Preprocessing.WebApi/TireOcr.Preprocessing.WebApi.csproj"
COPY . .
WORKDIR "/src/Preprocessing/TireOcr.Preprocessing.WebApi"
RUN dotnet build "TireOcr.Preprocessing.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TireOcr.Preprocessing.WebApi.csproj" -c Release -r linux-x64 -o /app/publish

FROM base AS final
RUN apt-get update
RUN apt-get install -y libc6 libicu-dev libfontconfig1
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=publish /app/publish/libSkiaSharp.so /usr/lib/
COPY --from=opencv-builder /usr/lib /usr/lib
# COPY --from=skiasharp-builder out/linux/x64/ /usr/lib

EXPOSE 8080
ENV DOTNET_VERSION=8.0.16 ASPNET_VERSION=8.0.16 APP_UID=1654 ASPNETCORE_HTTP_PORTS=8080 DOTNET_RUNNING_IN_CONTAINER=true
ENTRYPOINT ["dotnet", "TireOcr.Preprocessing.WebApi.dll"]
