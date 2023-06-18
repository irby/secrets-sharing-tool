FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env

RUN apt-get update && \
    apt-get install -y openssl && \
    openssl genrsa -des3 -passout pass:x -out server.pass.key 2048 && \
    openssl rsa -passin pass:x -in server.pass.key -out server.key && \
    rm server.pass.key && \
    openssl req -new -key server.key -out server.csr \
        -subj "/C=UK/ST=Warwickshire/L=Leamington/O=OrgName/OU=IT Department/CN=example.com" && \
    openssl x509 -req -days 365 -in server.csr -signkey server.key -out server.crt

WORKDIR /source

# Copy csproj and restore as distinct layers
COPY src/api .
RUN dotnet restore --use-current-runtime SecretsSharingTool.sln

# Copy everything else and build
COPY . .
RUN dotnet publish --use-current-runtime --self-contained false --no-restore -o /app

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 as runtime

WORKDIR /app
COPY --from=build-env /app .

ENTRYPOINT [ "dotnet", "SecretsSharingTool.Web.dll" ]
