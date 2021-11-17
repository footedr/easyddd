using System.Threading.Tasks;

namespace EasyDdd.ShipmentManagement.Core;

public interface IDispatchNumberService
{
	Task<DispatchNumber> ReserveNumber();
}