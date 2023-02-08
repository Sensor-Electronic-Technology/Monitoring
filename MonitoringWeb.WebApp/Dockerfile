FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["NuGet.config", "."]
COPY ["MonitoringWeb.WebApp/MonitoringWeb.WebApp.csproj", "MonitoringWeb.WebApp/"]
COPY ["MonitoringSystem.Shared/MonitoringSystem.Shared.csproj", "MonitoringSystem.Shared/"]
RUN dotnet restore "MonitoringWeb.WebApp/MonitoringWeb.WebApp.csproj"
COPY . .
WORKDIR "/src/MonitoringWeb.WebApp"
RUN dotnet build "MonitoringWeb.WebApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MonitoringWeb.WebApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MonitoringWeb.WebApp.dll"]
