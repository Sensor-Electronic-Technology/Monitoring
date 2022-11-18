FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["PingTest/PingTest.csproj", "PingTest/"]
RUN dotnet restore "PingTest/PingTest.csproj"
COPY . .
WORKDIR "/src/PingTest"
RUN dotnet build "PingTest.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PingTest.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PingTest.dll"]
