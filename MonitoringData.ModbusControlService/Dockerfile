FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MonitoringData.ModbusControlService/MonitoringData.ModbusControlService.csproj", "MonitoringData.ModbusControlService/"]
RUN dotnet restore "MonitoringData.ModbusControlService/MonitoringData.ModbusControlService.csproj"
COPY . .
WORKDIR "/src/MonitoringData.ModbusControlService"
RUN dotnet build "MonitoringData.ModbusControlService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MonitoringData.ModbusControlService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MonitoringData.ModbusControlService.dll"]
