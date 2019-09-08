#wait for the SQL Server to start up
sleep 25
#run the setup script to create the DB and the schema in the DB
cd DatabasesScripts
for filename in *.sql; do
    /opt/mssql-tools/bin/sqlcmd -S localhost -U services -P services -d master -i $filename
done
