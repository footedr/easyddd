namespace EasyDdd.Core
{
	public class LocationRequest
	{
		public ContactRequest Contact { get; set; } = default!;
		public AddressRequest Address { get; set; } = default!;
	}
}