rm -r Database/DatabasesScripts/
rm -r AuthAPI/Commonlibrary/
rm -r IntegrationAPI/Commonlibrary/
rm -r VLSUScheduleAPIs/Commonlibrary/

cp -r Commonlibrary AuthAPI/
cp -r Commonlibrary IntegrationAPI/
cp -r Commonlibrary VLSUScheduleAPIs/

cd AuthAPI/AuthAPI/
rm -r Migrations
dotnet ef migrations add Init
dotnet ef migrations script -v -o  ../../Database/DatabasesScripts/AuthScript.sql
cd ../..

cd VLSUScheduleAPIs/VLSUScheduleAPIs/
rm -r Migrations
dotnet ef migrations add Init
dotnet ef migrations script -v -o  ../../Database/DatabasesScripts/VLSUScheduleScript.sql
cd ../..