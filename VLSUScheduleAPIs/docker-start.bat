@ECHO OFF
FOR /f "tokens=*" %%i IN ('docker ps -q --filter "name=vlsu"') DO docker stop %%i
FOR /f "tokens=*" %%i IN ('docker ps -q --filter "name=dockercompose"') DO docker stop %%i
@ECHO ON
docker-compose up --force-recreate --abort-on-container-exit --build