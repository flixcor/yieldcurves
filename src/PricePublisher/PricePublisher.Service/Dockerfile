FROM mcr.microsoft.com/dotnet/core/aspnet:3.0.0-preview7-alpine3.9 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.0.100-preview6-alpine3.9 AS build
WORKDIR /src
COPY ["src/PricePublisher/PricePublisher.Service/PricePublisher.Service.csproj", "src/PricePublisher/PricePublisher.Service/"]
COPY ["src/Common/Common.Core/Common.Core.csproj", "src/Common/Common.Core/"]
COPY ["src/Common/Common.Infrastructure/Common.Infrastructure.csproj", "src/Common/Common.Infrastructure/"]
RUN dotnet restore "src/PricePublisher/PricePublisher.Service/PricePublisher.Service.csproj"
COPY . .
WORKDIR "/src/src/PricePublisher/PricePublisher.Service"
RUN dotnet build "PricePublisher.Service.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "PricePublisher.Service.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "PricePublisher.Service.dll"]
