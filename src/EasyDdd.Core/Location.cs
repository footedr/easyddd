using System;

namespace EasyDdd.Core
{
	public record Location(Address Address, Contact Contact)
	{
		[Obsolete("This is used by EF and is necessary due to issues w/ nested record types.")]
		private Location() : this(default!, default!)
		{
		}

		public LocationRequest ToDto()
		{
			return new LocationRequest
			{
				Address = Address.ToDto(),
				Contact = Contact.ToDto()
			};
		}
	};
}