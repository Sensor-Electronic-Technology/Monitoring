#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["NuGet.config", "."]
COPY ["MonitoringData.DataApi/MonitoringData.DataApi.csproj", "MonitoringData.DataApi/"]
RUN dotnet restore "MonitoringData.DataApi/MonitoringData.DataApi.csproj"
COPY . .
WORKDIR "/src/MonitoringData.DataApi"
RUN dotnet build "MonitoringData.DataApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MonitoringData.DataApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MonitoringData.DataApi.dll"]