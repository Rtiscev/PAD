# Base image for running the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

# Work as root to install packages
USER app

WORKDIR /app

# Install FFmpeg
RUN apt-get update \
    && apt install ffmpeg -y \
    && rm -rf /var/lib/apt/lists/*

# Build and publish steps
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# ARG to set build configuration
ARG BUILD_CONFIGURATION=Release

WORKDIR /src

# Copy the project file and restore dependencies
COPY ["MS_FFMpeg.csproj", "."]
RUN dotnet restore "./MS_FFMpeg.csproj"

# Copy everything else and build the project
COPY . .
RUN dotnet build "./MS_FFMpeg.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the app
FROM build AS publish
RUN dotnet publish "./MS_FFMpeg.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage setup - based on ASP.NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app


# Install FFmpeg
RUN apt-get update \
    && apt install -y ffmpeg iputils-ping \
    && rm -rf /var/lib/apt/lists/*

# Create the downloads directory and set permissions
RUN mkdir -p /app/downloads && chmod -R 777 /app/downloads

# Copy the published app from the build stage
COPY --from=publish /app/publish .

# Set the entrypoint for the application
ENTRYPOINT ["dotnet", "MS_FFMpeg.dll"]