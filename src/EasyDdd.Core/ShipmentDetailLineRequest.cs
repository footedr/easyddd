namespace EasyDdd.Core
{
	public record ShipmentDetailLineRequest(string FreightClass, int Weight, int HandlingUnitCount, string PackagingType, bool IsHazardous, string Description);
}