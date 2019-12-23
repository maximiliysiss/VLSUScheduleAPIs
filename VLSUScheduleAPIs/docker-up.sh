docker stop `docker ps -q --filter "name=dockercompose"`
docker stop `docker ps -q --filter "name=vlsu"`
docker-compose up --abort-on-container-exit --build