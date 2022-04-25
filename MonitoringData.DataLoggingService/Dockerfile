#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
RUN apt-get update && apt-get install -y --no-install-recommends gss-ntlmssp
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MonitoringData.DataLoggingService/MonitoringData.DataLoggingService.csproj", "MonitoringData.DataLoggingService/"]
COPY ["MonitoringSystem.Shared/MonitoringSystem.Shared.csproj", "MonitoringSystem.Shared/"]
COPY ["MonitoringData.Infrastructure/MonitoringData.Infrastructure.csproj", "MonitoringData.Infrastructure/"]
RUN dotnet restore "MonitoringData.DataLoggingService/MonitoringData.DataLoggingService.csproj"
COPY . .
WORKDIR "/src/MonitoringData.DataLoggingService"
RUN dotnet build "MonitoringData.DataLoggingService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MonitoringData.DataLoggingService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MonitoringData.DataLoggingService.dll"]