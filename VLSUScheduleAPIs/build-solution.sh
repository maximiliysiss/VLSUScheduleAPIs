rm -r AuthAPI/Commonlibrary/
rm -r IntegrationAPI/Commonlibrary/
rm -r VLSUScheduleAPIs/Commonlibrary/

cp -r Commonlibrary AuthAPI/
cp -r Commonlibrary IntegrationAPI/
cp -r Commonlibrary VLSUScheduleAPIs/

dotnet build