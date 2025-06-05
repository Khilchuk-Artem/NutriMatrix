FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /app
COPY . .
RUN dotnet restore "RecommendationService.Api.csproj"
RUN dotnet build "RecommendationService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build
RUN dotnet publish "RecommendationService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "RecommendationService.Api.dll"]