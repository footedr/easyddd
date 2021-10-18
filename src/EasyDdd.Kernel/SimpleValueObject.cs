using System;
using System.Runtime.CompilerServices;

namespace EasyDdd.Kernel
{
	public interface ISimpleValueObject<T>
	{
		T Value { get; }
	}

	public abstract class SimpleValueObject<T> : ValueObject, ISimpleValueObject<T>
	{
		protected SimpleValueObject(T value)
		{
			Value = value;
		}

		public T Value { get; }

		protected override ITuple AsTuple()
		{
			return ValueTuple.Create(Value);
		}

		public override string ToString()
		{
			return $"{GetType().Name} {Value?.ToString() ?? "(NULL)"}";
		}
	}
}