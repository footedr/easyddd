namespace EasyDdd.Core
{
	public class AddressRequest
	{
		public string Line1 { get; set; } = default!;
		public string? Line2 { get; set; }
		public string City { get; set; } = default!;
		public string StateAbbreviation { get; set; } = default!;
		public string PostalCode { get; set; } = default!;
	}
}