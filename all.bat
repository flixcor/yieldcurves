start docker-compose up &
start watch.run .\src\CurveRecipes\CurveRecipes.Service &
start watch.run .\src\CurveRecipes\CurveRecipes.Query.Service &
start watch.run .\src\MarketCurves\MarketCurves.Service &
start watch.run .\src\MarketCurves\MarketCurves.Query.Service &
start watch.run .\src\Instruments\Instruments.Service &
start watch.run .\src\Instruments\Instruments.Query.Service &
start npm run --prefix .\src\Presentation\MicroFrontends\curve-tool serve -- --port 8081