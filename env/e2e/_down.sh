#!/bin/bash
docker-compose -f docker-compose.yml -f env/e2e/docker-compose.e2e.yml down
