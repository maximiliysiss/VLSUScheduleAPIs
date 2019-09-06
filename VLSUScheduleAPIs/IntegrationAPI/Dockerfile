FROM microsoft/aspnetcore

COPY . /Services/
WORKDIR /Services/

CMD dotnet build

ENTRYPOINT [ "/bin/Debug/netcoreapp2.2/IntegrationAPI.dll" ]