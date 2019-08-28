set arg1=%1
cd %arg1%
cd client-app
CALL npm audit fix
CALL npm install
CALL npm run build --fix
cd ..
CALL dotnet run --no-build