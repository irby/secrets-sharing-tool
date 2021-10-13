#!/bin/bash
./wait-for-it.sh -s -t 150 $DB_HOST:$DB_PORT

if [[ $DB_ENGINE == 'mysql' ]]; then
  URL="jdbc:mysql://$DB_HOST:$DB_PORT/$DB_NAME?useSSL=false&allowPublicKeyRetrieval=true"
elif [[ $DB_ENGINE == 'sqlserver' ]]; then
  URL="jdbc:sqlserver://$DB_HOST:$DB_PORT;"
fi

sleep 2

echo "---------- running migrations ----------"
flyway -user=$DB_USER -password=$DB_PASSWORD -url=$URL -locations=filesystem:./migrations/kronocrypt/schema,filesystem:./migrations/kronocrypt/seeder migrate