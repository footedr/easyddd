using System.Threading.Tasks;

namespace EasyDdd.ShipmentManagement.Core;

public interface IShipmentIdService
{
	Task<ShipmentId> ReserveId();
}