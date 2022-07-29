#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["NuGet.config", "."]
COPY ["MonitoringSystem.ConfigApi/MonitoringSystem.ConfigApi.csproj", "MonitoringSystem.ConfigApi/"]
RUN dotnet restore "MonitoringSystem.ConfigApi/MonitoringSystem.ConfigApi.csproj"
COPY . .
WORKDIR "/src/MonitoringSystem.ConfigApi"
RUN dotnet build "MonitoringSystem.ConfigApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MonitoringSystem.ConfigApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MonitoringSystem.ConfigApi.dll"]