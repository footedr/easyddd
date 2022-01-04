namespace EasyDdd.Billing.Core;

/// <summary>
///     Represents a detail line on a shipment, an actual shipping commodity.
/// </summary>
/// <remarks>
///     In billing, modeling as a Value Object. Billing doesn't care about the identity or lifespan of a shipment detail line.
/// </remarks>
public record ShipmentDetail(string Class, int Weight, int HandlingUnitCount, bool IsHazardous);