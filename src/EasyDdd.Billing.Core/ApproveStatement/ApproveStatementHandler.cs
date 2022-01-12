using EasyDdd.Billing.Core.Specifications;
using EasyDdd.Kernel;
using Microsoft.Extensions.Logging;

namespace EasyDdd.Billing.Core.ApproveStatement
{
	public class ApproveStatementHandler : CommandHandler<ApproveStatementCommand, Statement>
	{
		private readonly NodaTime.IClock _clock;
		private readonly ILogger<ApproveStatementHandler> _logger;
		private readonly IRepository<Statement> _statementRepo;

		public ApproveStatementHandler(IRepository<Statement> statementRepo,
			NodaTime.IClock clock,
			ILogger<ApproveStatementHandler> logger)
		{
			_statementRepo = statementRepo;
			_clock = clock;
			_logger = logger;
		}

		public override async Task<Statement> Handle(ApproveStatementCommand command, CancellationToken cancellationToken)
		{
			_logger.LogInformation("Handling {EventType} for user: {User}.", nameof(ApproveStatementCommand), command.User.Identity?.Name);

			if (command.User.Identity?.Name == null)
			{
				throw new ArgumentNullException(nameof(command.User.Identity), "Creating a shipment requires a valid user.");
			}

			var statement = (await _statementRepo.FindAsync(new StatementByIdSpecification(command.StatementIdentifier))
					.ConfigureAwait(false))
				.SingleOrDefault();

			if (statement is null)
			{
				_logger.LogError("Unable to approve statement: {StatementId}. Statement not found.", command.StatementIdentifier);
				throw new NotFoundException<Statement>(command.StatementIdentifier);
			}

			statement.Processed(_clock.GetCurrentInstant());

			await _statementRepo.SaveAsync(statement)
				.ConfigureAwait(false);

			_logger.LogInformation("Statement with id: {StatementIdentifier} was approved.", command.StatementIdentifier);

			return statement;
		}
	}
}