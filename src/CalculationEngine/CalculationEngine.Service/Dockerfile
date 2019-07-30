FROM mcr.microsoft.com/dotnet/core/runtime:3.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["src/CalculationEngine/CalculationEngine.Service/CalculationEngine.Service.csproj", "src/CalculationEngine/CalculationEngine.Service/"]
COPY ["src/Common/Common.Core/Common.Core.csproj", "src/Common/Common.Core/"]
COPY ["src/Common/Common.Infrastructure/Common.Infrastructure.csproj", "src/Common/Common.Infrastructure/"]
RUN dotnet restore "src/CalculationEngine/CalculationEngine.Service/CalculationEngine.Service.csproj"
COPY . .
WORKDIR "/src/src/CalculationEngine/CalculationEngine.Service"
RUN dotnet build "CalculationEngine.Service.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "CalculationEngine.Service.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "CalculationEngine.Service.dll"]
