namespace EasyDdd.ShipmentManagement.Core
{
	public class ContactRequest
	{
		public string Name { get; set; } = default!;
		public string? Phone { get; set; }
		public string? Email { get; set; }
	}
}