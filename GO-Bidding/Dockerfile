# Base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj og nuget konfiguration
COPY nuget.config ./
COPY nuget-packages ./nuget-packages
COPY GO-Bidding/GO-Bidding.csproj GO-Bidding/

# Restore using local NuGet source
RUN dotnet restore "GO-Bidding/GO-Bidding.csproj" --configfile ./nuget.config

# Copy the rest of the code
COPY . .

# Build og publish
WORKDIR /src/GO-Bidding
RUN dotnet build GO-Bidding.csproj -c Release -o /app/build
RUN dotnet publish GO-Bidding.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "GO-Bidding.dll"]
