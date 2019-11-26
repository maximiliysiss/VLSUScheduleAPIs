docker stop `docker ps -qa`
docker-compose up --force-recreate --abort-on-container-exit --build