FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /vsdbg

ENV DOTNET_USE_POLLING_FILE_WATCHER 1

WORKDIR /app/Gateway/ApiGateway
ENTRYPOINT dotnet watch run --urls=https://+:443;http://+:80
