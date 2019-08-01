FROM mcr.microsoft.com/dotnet/core/sdk:3.0.100-preview6-alpine3.9 AS build
WORKDIR /vsdbg

ENV DOTNET_USE_POLLING_FILE_WATCHER 1

WORKDIR /app/CalculationEngine/CalculationEngine.Service
ENTRYPOINT dotnet watch run --urls=https://+:443;http://+:80
