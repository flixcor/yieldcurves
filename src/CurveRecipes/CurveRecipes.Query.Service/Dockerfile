FROM mcr.microsoft.com/dotnet/core/aspnet:3.0.0-preview7-alpine3.9 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0.100-preview6-alpine3.9 AS build
WORKDIR /src
COPY ["src/CurveRecipes/CurveRecipes.Query.Service/CurveRecipes.Query.Service.csproj", "src/CurveRecipes/CurveRecipes.Query.Service/"]
COPY ["src/Common/Common.Core/Common.Core.csproj", "src/Common/Common.Core/"]
COPY ["src/CurveRecipes/CurveRecipes.Domain/CurveRecipes.Domain.csproj", "src/CurveRecipes/CurveRecipes.Domain/"]
COPY ["src/Common/Common.Infrastructure/Common.Infrastructure.csproj", "src/Common/Common.Infrastructure/"]
RUN dotnet restore "src/CurveRecipes/CurveRecipes.Query.Service/CurveRecipes.Query.Service.csproj"
COPY . .
WORKDIR "/src/src/CurveRecipes/CurveRecipes.Query.Service"
RUN dotnet build "CurveRecipes.Query.Service.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "CurveRecipes.Query.Service.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "CurveRecipes.Query.Service.dll"]