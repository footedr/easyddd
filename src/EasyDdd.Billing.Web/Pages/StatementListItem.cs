using EasyDdd.Billing.Core;

namespace EasyDdd.Billing.Web.Pages;

public abstract record StatementListItem(StatementIdentifier Id,
	string BillToAccount,
	string BillToLocation,
	BillingPeriod BillingPeriod);

public record PendingStatementListItem(StatementIdentifier Id,
	string BillToAccount,
	string BillToLocation,
	BillingPeriod BillingPeriod,
	DateTime CreatedAt) : StatementListItem(Id, BillToAccount, BillToLocation, BillingPeriod);

public record ProcessedStatementListItem(StatementIdentifier Id,
	string BillToAccount,
	string BillToLocation,
	BillingPeriod BillingPeriod,
	DateTime ProcessedAt) : StatementListItem(Id, BillToAccount, BillToLocation, BillingPeriod);