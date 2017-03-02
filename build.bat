
rem dotnet restore

cd src/Module/Admin && dotnet build && cd ../../../
cd src/Module/Order && dotnet build && cd ../../../
cd src/Module/Search && dotnet build && cd ../../../

rem cd src/WebHost && npm install && npm install --global gulp-cli && gulp copy-module
cd src/WebHost && gulp copy-module && cd ../../

echo "Then type 'dotnet run' in src/WebHost to start the app."

pause