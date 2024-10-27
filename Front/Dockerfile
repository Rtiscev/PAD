FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER root 
RUN apt-get update -y && apt-get install -y iputils-ping
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Front.csproj", "."]
RUN dotnet restore "./Front.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./Front.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Front.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Front.dll"]