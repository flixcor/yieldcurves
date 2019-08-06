set arg1=%1
cd %arg1%
cd client-app
CALL npm install
CALL npm run build
cd ..
CALL dotnet watch run