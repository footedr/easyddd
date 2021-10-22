namespace EasyDdd.Core
{
	public record Address(string Line1, string City, string StateAbbreviation, string PostalCode)
	{
		public string? Line2 { get; init; }

		public AddressRequest ToDto()
		{
			return new AddressRequest
			{
				City = City,
				Line1 = Line1,
				Line2 = Line2,
				PostalCode = PostalCode,
				StateAbbreviation = StateAbbreviation
			};
		}
	}
}