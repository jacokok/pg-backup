# PG Backup

Backup your pg db from your front end

## Todo

- [ ] Create Web Api
  - [ ] Start backup
  - [ ] Backup status
  - [ ] List Backups
  - [ ] Remove backups
  - [ ] Send to cloud
  - [ ] Download backup locally
- [ ] pgdump or something to make db backup
- [ ] Upload backups to s3
- [ ] Run jobs on schedule
- [x] Run jobs out of process

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
