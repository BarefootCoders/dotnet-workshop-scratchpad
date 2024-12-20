# Pull the latest SQL Server image
docker pull mcr.microsoft.com/mssql/server:2022-latest

# Kill and remove any existing container named ContosoDb
docker kill ContosoDb
docker container rm ContosoDb

# Run a new SQL Server container
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=@#$%^Password0" --name 'ContosoDb' -p 1433:1433 -v /var/opt/mssql/backups -d mcr.microsoft.com/mssql/server:2022-latest

# Change SA password
docker exec -it ContosoDb /opt/mssql-tools18/bin/sqlcmd -S localhost -U SA -P "@#$%^Password0" -Q "ALTER LOGIN SA WITH PASSWORD='@#$%^SomePassword0'" -C