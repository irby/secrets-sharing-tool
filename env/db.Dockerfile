FROM boxfuse/flyway:5.2.4

COPY ./env/wait-for-it.sh ./wait-for-it.sh
COPY ./env/docker-entrypoint-db.sh ./docker-entrypoint-db.sh

RUN chmod 755 ./*.sh

COPY ./migrations ./migrations

ENTRYPOINT ["./docker-entrypoint-db.sh"]
