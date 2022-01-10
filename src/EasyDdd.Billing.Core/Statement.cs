using EasyDdd.Kernel;
using NodaTime;

namespace EasyDdd.Billing.Core;

public class Statement : Entity<StatementIdentifier>
{
	private readonly List<StatementLine> _lines = new List<StatementLine>();

	private Statement() : base(default!)
	{
		CustomerCode = default!;
		BillingPeriod = default!;
		BillToAccount = default!;
		BillToLocation = default!;
		CreatedAt = default!;
	}

	public Statement(StatementIdentifier identifier, 
		string customerCode, 
		BillingPeriod billingPeriod, 
		string billToAccount, 
		string billToLocation, 
		Instant createdAt) 
			: base(identifier)
	{
		CustomerCode = customerCode;
		BillingPeriod = billingPeriod;
		BillToAccount = billToAccount;
		BillToLocation = billToLocation;
		CreatedAt = createdAt;
	}

	public int Version { get; private set; }
	public string CustomerCode { get; }
	public BillingPeriod BillingPeriod { get; }
	public string BillToAccount { get; }
	public string BillToLocation { get; }
	public Instant CreatedAt { get; }
	public Instant? ProcessedAt { get; private set; }
	public IReadOnlyList<StatementLine> Lines => _lines;

	public void Processed(Instant processedAt)
	{
		ProcessedAt = processedAt;
		UpdateVersion();
	}

	public void AddLine(StatementLine line)
	{
		_lines.Add(line);
		UpdateVersion();
	}

	private void UpdateVersion()
	{
		Version++;
	}
}

public record BillingPeriod(LocalDate Start, LocalDate End);

public record StatementLine(string TmsNumber, string Description, int Quantity, decimal Amount, LocalDate TransactionDate)
{
	public int? HandlingUnits { get; init; }
	public string? Class { get; init; }
	public double? Weight { get; set; }
}