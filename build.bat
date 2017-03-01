
dotnet restore && dotnet build

cd src/WebHost && npm install && npm install --global gulp-cli && gulp copy-module && gulp copy-static

echo "Then type 'dotnet run' in src/WebHost to start the app."