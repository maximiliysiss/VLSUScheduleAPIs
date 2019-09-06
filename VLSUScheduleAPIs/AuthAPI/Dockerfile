FROM microsoft/aspnetcore:2.2

COPY . /Services/
WORKDIR /Services/

CMD dotnet build

ENTRYPOINT [ "/bin/Debug/netcore2.2/AuthAPI.dll" ]