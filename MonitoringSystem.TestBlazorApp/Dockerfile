#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
FROM nginx:alpine
COPY . /usr/share/nginx/html

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MonitoringSystem.TestBlazorApp/MonitoringSystem.TestBlazorApp.csproj", "MonitoringSystem.TestBlazorApp/"]
RUN dotnet restore "MonitoringSystem.TestBlazorApp/MonitoringSystem.TestBlazorApp.csproj"
COPY . .
WORKDIR "/src/MonitoringSystem.TestBlazorApp"
RUN dotnet build "MonitoringSystem.TestBlazorApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MonitoringSystem.TestBlazorApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MonitoringSystem.TestBlazorApp.dll"]