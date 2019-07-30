FROM mcr.microsoft.com/dotnet/core/aspnet:3.0.0-preview7-alpine3.9 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0.100-preview6-alpine3.9 AS build
WORKDIR /src
COPY ["src/Instruments/Instruments.Query.Service/Instruments.Query.Service.csproj", "src/Instruments/Instruments.Query.Service/"]
COPY ["src/Instruments/Instruments.Domain/Instruments.Domain.csproj", "src/Instruments/Instruments.Domain/"]
COPY ["src/Common/Common.Core/Common.Core.csproj", "src/Common/Common.Core/"]
COPY ["src/Common/Common.Infrastructure/Common.Infrastructure.csproj", "src/Common/Common.Infrastructure/"]
RUN dotnet restore "src/Instruments/Instruments.Query.Service/Instruments.Query.Service.csproj"
COPY . .
WORKDIR "/src/src/Instruments/Instruments.Query.Service"
RUN dotnet build "Instruments.Query.Service.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Instruments.Query.Service.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Instruments.Query.Service.dll"]
