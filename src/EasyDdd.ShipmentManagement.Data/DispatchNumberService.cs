using System.Data;
using System.Threading.Tasks;
using EasyDdd.ShipmentManagement.Core;
using Microsoft.EntityFrameworkCore;

namespace EasyDdd.ShipmentManagement.Data;

public class DispatchNumberService : IDispatchNumberService
{
	private readonly TmsContext _context;

	public DispatchNumberService(TmsContext context)
	{
		_context = context;
	}

	public async Task<DispatchNumber> ReserveNumber()
	{
		var conn = _context.Database.GetDbConnection();
		await using var command = conn.CreateCommand();

		command.CommandText = "select NEXT VALUE FOR ShipmentManagement.DispatchNumbers";

		if (conn.State != ConnectionState.Open) await conn.OpenAsync();

		var nextId = (await command.ExecuteScalarAsync().ConfigureAwait(false))?.ToString();
		return DispatchNumber.Create($"DSP{nextId}");
	}
}