# PG Backup

Backup your pg db from your front end

## Features

- API Backups
  - List
  - Download
  - Create
  - Delete
- Cloud Backups
  - Upload to S3
  - List
  - Download
  - Delete
- Run Jobs out of process
- Cron Schedule to run backups

## Todo

- [ ] Pack inside container
- [ ] Upload with CICD to dockerhub
- [ ] Update readme automagically
- [ ] Cognito Auth?

## Docker Run

```bash
docker run command goes here
```

## Docker Compose

```yaml
docker-compose: yaml
  should: go here
```

## Environment Variables

| Env                   | Description                            |
| --------------------- | -------------------------------------- |
| PG_AWS__AccessKey     | AWS Access Key                         |
| PG_AWS__SecretKey     | AWS Secret Key                         |
| PG_AWS__BucketName    | AWS Bucket Name                        |
| PG_Backup__Cron       | Backup cron: Default: "0 30 3 ? * SUN" |
| PG_DBConfig__Host     | Database host config                   |
| PG_DBConfig__Port     | Default: 5432                          |
| PG_DBConfig__Username | Database host                          |
| PG_DBConfig__Password | Database host                          |
| PG_DBConfig__Database | Database host                          |

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
