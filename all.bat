docker-compose pull &
start docker-compose up &
dotnet build &
start npm.install.dev .\src\Presentation\FrontEnd &
start full.stack.microservice .\src\CurveRecipes\CurveRecipes.Service &
start full.stack.microservice .\src\CurveRecipes\CurveRecipes.Query.Service &
start full.stack.microservice .\src\MarketCurves\MarketCurves.Service &
start full.stack.microservice .\src\MarketCurves\MarketCurves.Query.Service &
start full.stack.microservice .\src\Instruments\Instruments.Service &
start full.stack.microservice .\src\PricePublisher\PricePublisher.Service &
start full.stack.microservice .\src\PricePublisher\PricePublisher.Query.Service &
start full.stack.microservice .\src\CalculationEngine\CalculationEngine.Query.Service &
start cd.dotnet.run.bat .\src\CalculationEngine\CalculationEngine.Service
start cd.dotnet.run.bat .\src\Common\Common.EventStore.Server