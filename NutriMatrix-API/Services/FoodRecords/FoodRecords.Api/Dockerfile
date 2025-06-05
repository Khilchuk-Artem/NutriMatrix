FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /app
COPY . .
RUN dotnet restore "FoodRecords.Api.csproj"
RUN dotnet build "FoodRecords.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build
RUN dotnet publish "FoodRecords.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FoodRecords.Api.dll"]
