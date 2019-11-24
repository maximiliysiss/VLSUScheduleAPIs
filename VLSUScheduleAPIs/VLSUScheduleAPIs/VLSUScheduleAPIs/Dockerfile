FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS base
WORKDIR /app
EXPOSE 81

COPY . .
RUN dotnet restore "VLSUScheduleAPIs/VLSUScheduleAPIs/VLSUScheduleAPIs.csproj"
WORKDIR "VLSUScheduleAPIs/VLSUScheduleAPIs"
RUN dotnet build "VLSUScheduleAPIs.csproj" -c Release -o /build
RUN dotnet publish "VLSUScheduleAPIs.csproj" -c Release -o /publish

COPY migration.sh .
ENTRYPOINT ["./migration.sh", "VLSUScheduleAPIs.dll"]