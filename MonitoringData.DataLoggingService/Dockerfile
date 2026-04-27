#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
ENV TZ="America/New_York"

RUN apt-get update && apt-get install -y --no-install-recommends gss-ntlmssp
RUN apt-get install -y tzdata

RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MonitoringData.DataLoggingService/MonitoringData.DataLoggingService.csproj", "MonitoringData.DataLoggingService/"]
RUN dotnet restore "MonitoringData.DataLoggingService/MonitoringData.DataLoggingService.csproj"
COPY . .
WORKDIR "/src/MonitoringData.DataLoggingService"
RUN dotnet build "MonitoringData.DataLoggingService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MonitoringData.DataLoggingService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
#RUN rm dbSettings.json
#COPY --from=publish /app/publish/epi2settings.json ./dbSettings.json
ENTRYPOINT ["dotnet", "MonitoringData.DataLoggingService.dll"]
