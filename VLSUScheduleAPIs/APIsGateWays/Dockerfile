FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["APIsGateWays/APIsGateWays.csproj", "APIsGateWays/"]
RUN dotnet restore "APIsGateWays/APIsGateWays.csproj"
COPY . .
WORKDIR "/src/APIsGateWays"
RUN dotnet build "APIsGateWays.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "APIsGateWays.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "APIsGateWays.dll"]