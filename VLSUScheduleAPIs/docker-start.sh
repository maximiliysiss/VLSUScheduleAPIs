docker stop `docker ps -q --filter "name=dockercompose"`
docker stop `docker ps -q --filter "name=vlsu"`
docker-compose up --force-recreate --abort-on-container-exit --build