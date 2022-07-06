FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MonitoringWeb.ControlService/MonitoringWeb.ControlService.csproj", "MonitoringWeb.ControlService/"]
RUN dotnet restore "MonitoringWeb.ControlService/MonitoringWeb.ControlService.csproj"
COPY . .
WORKDIR "/src/MonitoringWeb.ControlService"
RUN dotnet build "MonitoringWeb.ControlService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MonitoringWeb.ControlService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MonitoringWeb.ControlService.dll"]
