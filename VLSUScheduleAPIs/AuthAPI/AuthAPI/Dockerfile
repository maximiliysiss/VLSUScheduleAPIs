FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS base
WORKDIR /src
EXPOSE 80

COPY . .
RUN dotnet restore "AuthAPI/AuthAPI/AuthAPI.csproj"
WORKDIR "AuthAPI/AuthAPI/"
RUN dotnet build "AuthAPI.csproj" -c Release -o /build
RUN dotnet publish "AuthAPI.csproj" -c Release -o /publish

COPY migration.sh .
RUN chmod +x migration.sh
COPY wait /wait
RUN chmod +x /wait
CMD /wait && sh ./migration.sh AuthAPI.dll