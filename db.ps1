#Set-ExecutionPolicy Unrestricted -Scope Process
docker pull mcr.microsoft.com/mssql/server:2022-latest

docker kill ContosoDb
docker container rm ContosoDb

docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=@#$%^Password0" --name 'ContosoDb' -p 1433:1433 -v /var/opt/mssql/backups -d mcr.microsoft.com/mssql/server:2022-latest

#change SA password
docker exec -it ContosoDb /opt/mssql-tools18/bin/sqlcmd -S localhost -U SA -P "@#$%^Password0" -Q "ALTER LOGIN SA WITH PASSWORD='@#$%^SomePassword0'" -C
