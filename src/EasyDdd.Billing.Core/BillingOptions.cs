namespace EasyDdd.Billing.Core;

public class BillingOptions
{
	public const string Billing = "Billing";

	public string CustomerCode { get; set; } = string.Empty;
	public string BillToAccount { get; set; } = string.Empty;
	public string BillToLocation { get; set; } = string.Empty;
	public decimal TransactionFee { get; set; } = 0.0M;
}