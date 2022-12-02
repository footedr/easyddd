# Easy Domain Driven Design

## Running in Docker

```
docker build -t shipmentmanagement -f src/EasyDdd.ShipmentManagement.Web/Dockerfile .
docker build -t billing -f src/EasyDdd.Billing.Web/Dockerfile .
```
Then...
```
docker-compose up -d
```

## Debug out of Visual Studio
```
docker-compose start zookeeper
docker-compose start kafka
```
AND
```
docker-compose start sqlserver
```

### Open solution in VS2022 and run projects from there.