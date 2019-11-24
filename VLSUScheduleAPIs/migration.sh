#!/bin/bash
dotnet ef migrations add init_$(cat /dev/urandom | tr -dc 'a-za-z0-9' | fold -w 32 | head -n 1)
dotnet ef database update

cd /publish
rm -r /src
dotnet $1