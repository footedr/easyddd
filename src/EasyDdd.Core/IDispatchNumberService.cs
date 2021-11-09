using System.Threading.Tasks;

namespace EasyDdd.Core;

public interface IDispatchNumberService
{
	Task<DispatchNumber> ReserveNumber();
}