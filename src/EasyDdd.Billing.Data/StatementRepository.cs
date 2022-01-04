using System.Data;
using EasyDdd.Billing.Core;
using EasyDdd.Kernel;
using Microsoft.EntityFrameworkCore;

namespace EasyDdd.Billing.Data;

public class StatementRepository : IStatementRepository
{
	private readonly BillingContext _context;

	public StatementRepository(BillingContext context)
	{
		_context = context;
	}

	public async Task<IReadOnlyList<Statement>> FindAsync(Specification<Statement> specification)
	{
		var results = await _context.Set<Statement>()
			.AsTracking()
			.Include(s => s.Lines)
			.Where(specification.ToExpression())
			.ToListAsync();

		return results;
	}

	public async Task SaveAsync(Statement entity)
	{
		var state = _context.Entry(entity)?.State ?? EntityState.Detached;

		if (state == EntityState.Detached)
			_context.Add(entity);
		else
			_context.Update(entity);

		await _context.SaveChangesAsync();
	}

	public async Task DeleteAsync(Statement entity)
	{
		_context.Remove(entity);
		await _context.SaveChangesAsync();
	}

	public async Task<StatementIdentifier> ReserveStatementIdentifier()
	{
		var conn = _context.Database.GetDbConnection();
		using var command = conn.CreateCommand();

		command.CommandText = "select NEXT VALUE FOR billing.StatementIdentifiers";

		if (conn.State != ConnectionState.Open) await conn.OpenAsync();

		var nextId = (await command.ExecuteScalarAsync().ConfigureAwait(false))?.ToString();
		return StatementIdentifier.Create(nextId);
	}
}