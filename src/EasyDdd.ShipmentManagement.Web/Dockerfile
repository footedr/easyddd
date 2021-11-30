FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS source
# Ideally each set of Web/Core/Data projects would be there own "microservice" so COPY . . would be ok.
# In this case, however, this is copying unnecessary source code into the image.
COPY . .

FROM source as restore
RUN dotnet restore

FROM restore as build
RUN dotnet build -c Release --no-restore

FROM build as publish
RUN dotnet publish src/EasyDdd.ShipmentManagement.Web -c Release -o /app/publish --no-build

FROM base as final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EasyDdd.ShipmentManagement.Web.dll"]