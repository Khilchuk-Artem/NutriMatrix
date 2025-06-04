# Use full SDK to build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release

# Set working directory
WORKDIR /app

# Copy everything from repo root (adjust context via railway.toml)
COPY . .

# Restore and build
WORKDIR /app/Services/Recommendation/RecommendationService.Api
RUN dotnet restore "RecommendationService.Api.csproj"
RUN dotnet build "RecommendationService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish
RUN dotnet publish "RecommendationService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "RecommendationService.Api.dll"]
