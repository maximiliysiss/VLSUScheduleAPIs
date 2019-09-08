FROM mcr.microsoft.com/dotnet/core/aspnet:2.2
FROM microsoft/dotnet:2.2-sdk

COPY . /Services/VLSUScheduleAPI/
WORKDIR /Services/VLSUScheduleAPI/VLSUScheduleAPIs

RUN dotnet publish -o /bin/pub/

ENTRYPOINT [ "dotnet" ,"/bin/pub/VLSUScheduleAPIs.dll" ]