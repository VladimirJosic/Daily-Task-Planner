# ===== BUILD STAGE =====
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# The whole repository is copied, because the API project references the other
# three projects of the solution.
COPY . .

WORKDIR /src/backend/DailyTaskPlaner.Api
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

# ===== RUNTIME STAGE =====
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "DailyTaskPlaner.Api.dll"]
