FROM mcr.microsoft.com/dotnet/core/aspnet:2.2
FROM microsoft/dotnet:2.2-sdk

COPY . /GateWays/
WORKDIR /GateWays/

RUN dotnet publish -o /bin/pub/

ENTRYPOINT [ "dotnet" ,"/bin/pub/APIsGateWays.dll" ]