docker stop `docker ps -aq --filter "name=dockercompose"`
docker stop `docker ps -aq --filter "name=vlsu"`
docker-compose up --force-recreate --abort-on-container-exit --build