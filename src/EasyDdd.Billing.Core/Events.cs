using EasyDdd.Kernel;

namespace EasyDdd.Billing.Core
{
	public abstract record BillingDomainEvent(StatementIdentifier StatementIdentifier) : DomainEvent
	{
		public static string BoundedContextName => "Billing";
		public static string CollectionName => "Statements";

		public override AggregateIdentifier GetAggregateIdentifier() =>
			new AggregateIdentifier(BoundedContextName, CollectionName, StatementIdentifier.Value);
	}

	public record StatementCreated(Statement Statement) : BillingDomainEvent(Statement.Identifier);
	public record StatementLineAdded(StatementIdentifier StatementIdentifier, StatementLine StatementLine) : BillingDomainEvent(StatementIdentifier);
}