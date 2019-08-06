start docker-compose up &
start full.stack.microservice .\src\CurveRecipes\CurveRecipes.Service &
start full.stack.microservice .\src\CurveRecipes\CurveRecipes.Query.Service &
start full.stack.microservice .\src\MarketCurves\MarketCurves.Service &
start full.stack.microservice .\src\MarketCurves\MarketCurves.Query.Service &
start full.stack.microservice .\src\Instruments\Instruments.Service &
start full.stack.microservice .\src\Instruments\Instruments.Query.Service &
start npm.install.serve .\src\Presentation\MicroFrontends\curve-tool