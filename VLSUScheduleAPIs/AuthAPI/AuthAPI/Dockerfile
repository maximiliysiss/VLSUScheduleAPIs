FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS base
WORKDIR /src
EXPOSE 80

COPY . .
RUN dotnet restore "AuthAPI/AuthAPI/AuthAPI.csproj"
WORKDIR "AuthAPI/AuthAPI/"
RUN dotnet build "AuthAPI.csproj" -c Release -o /build
RUN dotnet publish "AuthAPI.csproj" -c Release -o /publish

COPY migration.sh .
ENTRYPOINT ["./migration.sh", "AuthAPI.dll"]