FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER root
WORKDIR /app
EXPOSE 80
EXPOSE 443

ENV TZ="America/New_York"

RUN apt-get update && apt-get install -y --no-install-recommends gss-ntlmssp
RUN apt-get install -y tzdata

RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MonitoringData.DataLoggingService/MonitoringData.DataLoggingService.csproj", "MonitoringData.DataLoggingService/"]
COPY ["MonitoringData.Infrastructure/MonitoringData.Infrastructure.csproj", "MonitoringData.Infrastructure/"]
COPY ["MonitoringSystem.Shared/MonitoringSystem.Shared.csproj", "MonitoringSystem.Shared/"]
RUN dotnet restore "MonitoringData.DataLoggingService/MonitoringData.DataLoggingService.csproj"
COPY . .
WORKDIR "/src/MonitoringData.DataLoggingService"
RUN dotnet build "./MonitoringData.DataLoggingService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MonitoringData.DataLoggingService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MonitoringData.DataLoggingService.dll"]
