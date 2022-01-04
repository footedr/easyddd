using System;

namespace EasyDdd.Kernel
{
	public class NotFoundException<T> : Exception
	{
		public NotFoundException(string? identifier) : base($"Could not find {typeof(T).Name} with identifier {identifier ?? "(NULL)"}") { }
	}
}