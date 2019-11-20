FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["AuthAPI/AuthAPI/AuthAPI.csproj", "AuthAPI/AuthAPI/"]
COPY ["ControllerCommon/CommonLibrary.csproj", "ControllerCommon/"]
RUN dotnet restore "AuthAPI/AuthAPI/AuthAPI.csproj"
COPY . .
WORKDIR "/src/AuthAPI/AuthAPI"
RUN dotnet build "AuthAPI.csproj" -c Release -o /app
RUN dotnet ef migrations add init
RUN dotnet ef database update

FROM build AS publish
RUN dotnet publish "AuthAPI.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "AuthAPI.dll"]