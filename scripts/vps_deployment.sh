#!/bin/bash

echo "Current working directory is:"
pwd

cd /var/www/html/admin.bejebeje.com/

echo "Now the current working directory is:"
pwd

echo "stopping docker container"
docker stop bejebeje-admin

echo "removing old container"
docker container rm bejebeje-admin

echo "pull latest docker image"
docker pull bejebeje/admin

echo "run latest docker image"
docker run -d -p 5025:5000 --name bejebeje-admin --network bejebeje_net --env-file ./variables.env bejebeje/admin