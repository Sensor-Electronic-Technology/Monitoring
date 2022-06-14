#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["NuGet.config", "."]
COPY ["MonitoringWeb.WebAppV2/MonitoringWeb.WebAppV2.csproj", "MonitoringWeb.WebAppV2/"]
RUN dotnet restore "MonitoringWeb.WebAppV2/MonitoringWeb.WebAppV2.csproj"
COPY . .
WORKDIR "/src/MonitoringWeb.WebAppV2"
RUN dotnet build "MonitoringWeb.WebAppV2.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MonitoringWeb.WebAppV2.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MonitoringWeb.WebAppV2.dll"]