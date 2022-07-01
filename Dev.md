# Development

## Commands

```bash
# run in docker/podman
docker run -it --rm docker.io/postgres /bin/bash
# set password env var
export PGPASSWORD=postgres
# backup db
pg_dump -h host.containers.internal -U postgres -Fc -v dbname > dump.sql
# drop db
dropdb -h host.containers.internal -U postgres -w bak
# new db
createdb -h host.containers.internal -U postgres -w bak
# restore db
pg_restore -h host.containers.internal -U postgres -d bak -v < dump.sql
```

## Alias

```bash
echo -en '#!/bin/bash \ndocker run -it -e PGPASSWORD=postgres --rm docker.io/postgres pg_dump $@' | sudo tee -a /usr/bin/pg_dump
sudo chmod +x /usr/bin/pg_dump
```

## Secrets

```bash
dotnet user-secrets init
dotnet user-secrets set "AWS:AccessKey" "secret"
dotnet user-secrets set "AWS:SecretKey" "secret"
dotnet user-secrets set "AWS:BucketName" "secret"
dotnet user-secrets list
```
