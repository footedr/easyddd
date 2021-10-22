using NodaTime;

namespace EasyDdd.Core
{
	public class AppointmentWindowRequest
	{
		public LocalDate Date { get; set; }
		public string Start { get; set; } = default!;
		public string End { get; set; } = default!;
	}
}