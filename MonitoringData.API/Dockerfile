#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MonitoringData.API/MonitoringData.API.csproj", "MonitoringData.API/"]
RUN dotnet restore "MonitoringData.API/MonitoringData.API.csproj"
COPY . .
WORKDIR "/src/MonitoringData.API"
RUN dotnet build "MonitoringData.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MonitoringData.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MonitoringData.API.dll"]