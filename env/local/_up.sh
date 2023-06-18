#!/bin/bash
docker-compose -f docker-compose.yml -f env/local/docker-compose.local.yml up --build
