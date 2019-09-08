rm -r AuthAPI/Commonlibrary/
rm -r IntegrationAPI/Commonlibrary/
rm -r VLSUScheduleAPIs/Commonlibrary/

cp -r Commonlibrary AuthAPI/
cp -r Commonlibrary IntegrationAPI/
cp -r Commonlibrary VLSUScheduleAPIs/

cd AuthAPI/AuthAPI/
dotnet ef migrations script -v -o  ../../Database/DatabasesScripts/AuthScript.sql
cd ../..

cd IntegrationAPI/IntegrationAPI/
dotnet ef migrations script -v -o  ../../Database/DatabasesScripts/IntegrationScript.sql
cd ../..

cd VLSUScheduleAPIs/VLSUScheduleAPIs/
dotnet ef migrations script -v -o  ../../Database/DatabasesScripts/VLSUScheduleScript.sql
cd ../..

docker-compose build