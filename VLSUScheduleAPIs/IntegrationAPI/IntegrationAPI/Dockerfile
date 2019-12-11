FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["IntegrationAPI/IntegrationAPI/IntegrationAPI.csproj", "IntegrationAPI/IntegrationAPI/"]
COPY ["NetServiceConnection/NetServiceConnection.csproj", "NetServiceConnection/"]
COPY ["ControllerCommon/CommonLibrary.csproj", "ControllerCommon/"]
RUN dotnet restore "IntegrationAPI/IntegrationAPI/IntegrationAPI.csproj"
COPY . .
WORKDIR "/src/IntegrationAPI/IntegrationAPI"
RUN dotnet build "IntegrationAPI.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "IntegrationAPI.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
COPY wait /wait
RUN chmod +x /wait
CMD /wait && dotnet IntegrationAPI.dll