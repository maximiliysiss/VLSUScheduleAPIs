@ECHO OFF
FOR /f "tokens=*" %%i IN ('docker ps -aq --filter "name=vlsu"') DO docker stop %%i
FOR /f "tokens=*" %%i IN ('docker ps -aq --filter "name=dockercompose"') DO docker stop %%i