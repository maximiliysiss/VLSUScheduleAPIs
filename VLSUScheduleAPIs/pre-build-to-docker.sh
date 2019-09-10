rm -r AuthAPI/Commonlibrary/
rm -r IntegrationAPI/Commonlibrary/
rm -r VLSUScheduleAPIs/Commonlibrary/

cp -r Commonlibrary AuthAPI/
cp -r Commonlibrary IntegrationAPI/
cp -r Commonlibrary VLSUScheduleAPIs/

cd AuthAPI/AuthAPI/
rm -r Migrations
cd ../..

cd VLSUScheduleAPIs/VLSUScheduleAPIs/
rm -r Migrations
cd ../..
