@ECHO OFF
FOR /f "tokens=*" %%i IN ('docker ps -q') DO docker stop %%i
@ECHO ON
docker-compose up --force-recreate --abort-on-container-exit --build