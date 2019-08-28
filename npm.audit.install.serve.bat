set arg1=%1
cd %arg1%
npm audit fix && npm install && npm run serve