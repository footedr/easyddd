using NodaTime;

namespace EasyDdd.Web.Pages.Shipments
{
	public class LocationRequest
	{
		public ContactRequest Contact { get; set; } = default!;
		public AddressRequest Address { get; set; } = default!;
	}

	public class ContactRequest
	{
		public string CompanyName { get; set; } = default!;
		public string? Phone { get; set; }
		public string? Email { get; set; }
	}

	public class AddressRequest
	{
		public string Line1 { get; set; } = default!;
		public string? Line2 { get; set; }
		public string City { get; set; } = default!;
		public string StateAbbreviation { get; set; } = default!;
		public string PostalCode { get; set; } = default!;
	}

	public class AppointmentWindowRequest
	{
		public LocalDate Date { get; set; }
		public LocalTime Start { get; set; }
		public LocalTime End { get; set; }
	}
}