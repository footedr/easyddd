using System.Data;
using System.Threading.Tasks;
using EasyDdd.ShipmentManagement.Core;
using Microsoft.EntityFrameworkCore;

namespace EasyDdd.ShipmentManagement.Data;

public class ShipmentIdService : IShipmentIdService
{
	private readonly TmsContext _context;

	public ShipmentIdService(TmsContext context)
	{
		_context = context;
	}

	public async Task<ShipmentId> ReserveId()
	{
		var conn = _context.Database.GetDbConnection();
		await using var command = conn.CreateCommand();

		command.CommandText = "select NEXT VALUE FOR ShipmentManagement.ShipmentIds";

		if (conn.State != ConnectionState.Open) await conn.OpenAsync();

		var nextId = (await command.ExecuteScalarAsync().ConfigureAwait(false))?.ToString();
		return ShipmentId.Create($"TMS{nextId}");
	}
}