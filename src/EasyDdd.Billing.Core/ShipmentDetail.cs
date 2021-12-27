namespace EasyDdd.Billing.Core;

public record ShipmentDetail(string Class, int Weight, int HandlingUnitCount, bool IsHazardous);