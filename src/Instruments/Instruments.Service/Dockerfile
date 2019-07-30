FROM mcr.microsoft.com/dotnet/core/aspnet:3.0.0-preview7-alpine3.9 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0.100-preview6-alpine3.9 AS build
WORKDIR /src
COPY ["src/Instruments/Instruments.Service/Instruments.Service.csproj", "src/Instruments/Instruments.Service/"]
COPY ["src/Instruments/Instruments.Domain/Instruments.Domain.csproj", "src/Instruments/Instruments.Domain/"]
COPY ["src/Common/Common.Core/Common.Core.csproj", "src/Common/Common.Core/"]
RUN dotnet restore "src/Instruments/Instruments.Service/Instruments.Service.csproj"
COPY . .
WORKDIR "/src/src/Instruments/Instruments.Service"
RUN dotnet build "Instruments.Service.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Instruments.Service.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Instruments.Service.dll"]
