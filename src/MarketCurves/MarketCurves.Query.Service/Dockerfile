FROM mcr.microsoft.com/dotnet/core/aspnet:3.0.0-preview7-alpine3.9 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0.100-preview6-alpine3.9 AS build
WORKDIR /src
COPY ["src/MarketCurves/MarketCurves.Query.Service/MarketCurves.Query.Service.csproj", "src/MarketCurves/MarketCurves.Query.Service/"]
COPY ["src/Common/Common.Core/Common.Core.csproj", "src/Common/Common.Core/"]
COPY ["src/Common/Common.Infrastructure/Common.Infrastructure.csproj", "src/Common/Common.Infrastructure/"]
COPY ["src/MarketCurves/MarketCurves.Domain/MarketCurves.Domain.csproj", "src/MarketCurves/MarketCurves.Domain/"]
RUN dotnet restore "src/MarketCurves/MarketCurves.Query.Service/MarketCurves.Query.Service.csproj"
COPY . .
WORKDIR "/src/src/MarketCurves/MarketCurves.Query.Service"
RUN dotnet build "MarketCurves.Query.Service.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "MarketCurves.Query.Service.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "MarketCurves.Query.Service.dll"]
