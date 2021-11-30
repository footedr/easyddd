FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 60606 60607

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS source
COPY . .

FROM source as restore
RUN dotnet restore

FROM restore as build
RUN dotnet build -c Release --no-restore

FROM build AS publish
RUN dotnet publish src/AzureEventGridSimulator -c Release -o /app/publish --no-build

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AzureEventGridSimulator.dll"]