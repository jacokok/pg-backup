# PG Backup

Backup your postgres database from your front end using this api

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
- Logs
  - View
  - Delete

## Docker Run

```bash
docker run doink/pg-backup:latest -p 5000:5000 -d -e PG_DBConfig__Database="test"

docker run -p 5000:5000 -e PG_DBConfig__LogsConnectionString="User ID=postgres;Password=postgres;Host=host.containers.internal;Port=5432;Database=logs" -e PG_DBConfig__Host="host.containers.internal" pg-backup 
```

## Docker Compose

```yaml
version: "3.7"
services:
  pg-backup:
    image: docker.io/doink/pg-backup:latest
    hostname: pg-backup
    container_name: pg-backup
    restart: always
    ports:
      - 5000:5000
    environment:
      PG_DBConfig__Host: "postgres"
      PG_DBConfig__Username: "postgres"
      PG_DBConfig__Password: "postgres"
      PG_DBConfig__Database: "test"
      # Cloud config
      PG_AWS__AccessKey: ""
      PG_AWS__SecretKey: "",
      PG_AWS__BucketName: ""
  postgres:
    image: docker.io/postgres:14
    hostname: postgres
    container_name: postgres
    restart: always
    volumes:
      - db_data:/var/lib/postgresql/data
    ports:
      - 5432:5432
    environment:
      POSTGRES_USER: "postgres"
      POSTGRES_PASSWORD: "postgres"
      POSTGRES_DB: "test"
volumes:
  db_data:
```

## Environment Variables

| Env                               | Description                            |
| --------------------------------- | -------------------------------------- |
| PG_AWS__AccessKey                 | AWS Access Key                         |
| PG_AWS__SecretKey                 | AWS Secret Key                         |
| PG_AWS__BucketName                | AWS Bucket Name                        |
| PG_Backup__Cron                   | Backup cron: Default: "0 30 3 ? * SUN" |
| PG_DBConfig__Host                 | Database host config                   |
| PG_DBConfig__Port                 | Default: 5432                          |
| PG_DBConfig__Username             | Database host                          |
| PG_DBConfig__Password             | Database host                          |
| PG_DBConfig__Database             | Database host                          |
| PG_DBConfig__LogsConnectionString | Connection string to logs              |
| PG_DBConfig__LogTable             | Log table name                         |
