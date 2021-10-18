using NodaTime;

namespace EasyDdd.Core
{
	public record AppointmentWindow(LocalDate Date, LocalTime Start, LocalTime End);
}