#!/bin/bash

docker-compose --project-directory . -f env/local/docker-compose-db.local.yml up --build

# This script starts a local container build. This is the opposite of _down.sh
