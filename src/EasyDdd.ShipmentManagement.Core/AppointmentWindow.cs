using NodaTime;

namespace EasyDdd.ShipmentManagement.Core
{
	public record AppointmentWindow(LocalDate Date, LocalTime Start, LocalTime End)
	{
		public AppointmentWindowRequest ToDto()
		{
			return new AppointmentWindowRequest
			{
				Date = Date,
				Start = $"{Start:hh:mm tt}",
				End = $"{End:hh:mm tt}"
			};
		}
	}
}