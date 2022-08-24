#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["NuGet.config", "."]
COPY ["MonitoringWeb.ConfigTesting/MonitoringWeb.ConfigTesting.csproj", "MonitoringWeb.ConfigTesting/"]
RUN dotnet restore "MonitoringWeb.ConfigTesting/MonitoringWeb.ConfigTesting.csproj"
COPY . .
WORKDIR "/src/MonitoringWeb.ConfigTesting"
RUN dotnet build "MonitoringWeb.ConfigTesting.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MonitoringWeb.ConfigTesting.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MonitoringWeb.ConfigTesting.dll"]