namespace EasyDdd.Core
{
	public record Location(Address Address, Contact Contact);

	public record Contact(string Name)
	{
		public string? CompanyName { get; init; }
		public string? Phone { get; init; }
		public string? Email { get; init; }
	}

	public record Address(string Line1, string City, string State, string PostalCode)
	{
		public string? Line2 { get; init; }
	}
}