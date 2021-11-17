namespace EasyDdd.ShipmentManagement.Core
{
	public record Contact(string Name)
	{
		public string? Phone { get; init; }
		public string? Email { get; init; }

		public ContactRequest ToDto()
		{
			return new ContactRequest
			{
				Email = Email,
				Name = Name,
				Phone = Phone
			};
		}
	}
}