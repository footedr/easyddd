using System;

namespace EasyDdd.Kernel
{
	public class NotFoundException : Exception
	{
		public NotFoundException(string message)
			: base(message)
		{
		}
	}
}