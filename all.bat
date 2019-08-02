start docker-compose up &
start watch.run .\src\CurveRecipes\CurveRecipes.Service &
start watch.run .\src\CurveRecipes\CurveRecipes.Query.Service &
start watch.run .\src\MarketCurves\MarketCurves.Service &
start watch.run .\src\MarketCurves\MarketCurves.Query.Service &
start watch.run .\src\Instruments\Instruments.Service &
start watch.run .\src\Instruments\Instruments.Query.Service &
start npm run --prefix .\src\Presentation\MicroFrontends\curve-tool serve -- --port 8081 &
start npm run --prefix .\src\CurveRecipes\CurveRecipes.Service\client-app build &
start npm run --prefix .\src\CurveRecipes\CurveRecipes.Query.Service\client-app build &
start npm run --prefix .\src\MarketCurves\MarketCurves.Service\client-app build &
start npm run --prefix .\src\MarketCurves\MarketCurves.Query.Service\client-app build &
start npm run --prefix .\src\Instruments\Instruments.Service\client-app build &
start npm run --prefix .\src\Instruments\Instruments.Query.Service\client-app build