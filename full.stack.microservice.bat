set arg1=%1
cd %arg1%
CALL npm install
CALL npm run build -- --fix
CALL dotnet run --no-build