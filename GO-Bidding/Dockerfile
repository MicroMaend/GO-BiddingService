# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /app

# Copy project and solution files
COPY . .

# Create a local NuGet packages directory and copy the .nupkg before restore
RUN mkdir -p /app/nuget-packages
COPY GOCore.1.0.11.nupkg /app/nuget-packages/

# Copy nuget.config into place
COPY nuget.config ./nuget.config

# Restore using custom nuget.config
RUN dotnet restore --configfile nuget.config

# Publish
RUN dotnet publish -c Release -o /app/published-app

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
WORKDIR /app
COPY --from=build /app/published-app /app
ENTRYPOINT ["dotnet", "/app/GO-Bidding.dll"]