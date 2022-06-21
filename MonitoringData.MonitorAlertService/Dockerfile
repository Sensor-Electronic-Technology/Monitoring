FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MonitoringData.MonitorAlertService/MonitoringData.MonitorAlertService.csproj", "MonitoringData.MonitorAlertService/"]
RUN dotnet restore "MonitoringData.MonitorAlertService/MonitoringData.MonitorAlertService.csproj"
COPY . .
WORKDIR "/src/MonitoringData.MonitorAlertService"
RUN dotnet build "MonitoringData.MonitorAlertService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MonitoringData.MonitorAlertService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MonitoringData.MonitorAlertService.dll"]
