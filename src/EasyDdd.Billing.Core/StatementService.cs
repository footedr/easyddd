using EasyDdd.Billing.Core.Specifications;
using EasyDdd.Kernel;
using NodaTime;

namespace EasyDdd.Billing.Core;

public class StatementService
{
	private readonly NodaTime.IClock _clock;
	private readonly IStatementRepository _statementRepository;

	public StatementService(IStatementRepository statementRepository, NodaTime.IClock clock)
	{
		_statementRepository = statementRepository;
		_clock = clock;
	}

	public async Task<Statement> GetOpenStatement(string customerCode, string billToAccount, string billToLocation, LocalDate transactionDate)
	{
		var currentStatement = await _statementRepository.FindAsync(new PendingStatementSpecification(customerCode, billToAccount, billToLocation))
			.SingleOrDefaultAsync()
			.ConfigureAwait(false);

		if (currentStatement != null) return currentStatement;

		var statementId = await _statementRepository.ReserveStatementIdentifier();
		var startDate = new LocalDate(transactionDate.Year, transactionDate.Month, 1);
		var endDate = startDate.PlusMonths(1);

		currentStatement = new Statement(
			statementId,
			customerCode,
			new BillingPeriod(startDate, endDate),
			billToAccount,
			billToLocation,
			_clock.GetCurrentInstant());

		return currentStatement;
	}

	public async Task<Statement> AddLineToOpenStatement(string customerCode, string billToAccount, string billToLocation, StatementLine line)
	{
		var statement = await GetOpenStatement(customerCode, billToAccount, billToLocation, line.TransactionDate);
		statement.AddLine(line);
		await _statementRepository.SaveAsync(statement);
		return statement;
	}
}