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

## Temp commands

docker run -it --rm postgres pg_dump --version
docker run -it --rm postgres pg_dump dbname -U username -h localhost -F c > backup
docker run -it --rm postgres pg_restore -h localhost -p 5432 -U username -d test -v backup