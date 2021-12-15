FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 60606 60607

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS source
COPY src/AzureEventGridSimulator src/AzureEventGridSimulator

FROM source as restore
RUN dotnet restore src/AzureEventGridSimulator/AzureEventGridSimulator.csproj

FROM restore as build
RUN dotnet build src/AzureEventGridSimulator/AzureEventGridSimulator.csproj -c Release --no-restore

FROM build AS publish
RUN dotnet publish src/AzureEventGridSimulator -c Release -o /app/publish --no-build

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AzureEventGridSimulator.dll"]