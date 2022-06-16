FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MonitoringTesting.TestAlertService/MonitoringTesting.TestAlertService.csproj", "MonitoringTesting.TestAlertService/"]
RUN dotnet restore "MonitoringTesting.TestAlertService/MonitoringTesting.TestAlertService.csproj"
COPY . .
WORKDIR "/src/MonitoringTesting.TestAlertService"
RUN dotnet build "MonitoringTesting.TestAlertService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MonitoringTesting.TestAlertService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MonitoringTesting.TestAlertService.dll"]
