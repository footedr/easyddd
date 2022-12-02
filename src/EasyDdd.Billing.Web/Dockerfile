FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS source
COPY src/EasyDdd.Kernel src/EasyDdd.Kernel
COPY src/EasyDdd.Billing.Core src/EasyDdd.Billing.Core
COPY src/EasyDdd.Billing.Data src/EasyDdd.Billing.Data
COPY src/EasyDdd.Billing.Web src/EasyDdd.Billing.Web

FROM source as restore
RUN dotnet restore src/EasyDdd.Billing.Web/EasyDdd.Billing.Web.csproj

FROM restore as build
RUN dotnet build src/EasyDdd.Billing.Web/EasyDdd.Billing.Web.csproj -c Release --no-restore

FROM build as publish
RUN dotnet publish src/EasyDdd.Billing.Web -c Release -o /app/publish --no-build

FROM base as final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EasyDdd.ShipmentManagement.Web.dll"]