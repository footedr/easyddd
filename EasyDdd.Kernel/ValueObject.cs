using System;
using System.Runtime.CompilerServices;

namespace EasyDdd.Kernel
{
	public abstract class ValueObject : IEquatable<ValueObject>
	{
		public bool Equals(ValueObject? other)
		{
			if (other is null) return false;

			if (GetType() != other.GetType()) return false;
			;

			return AsTuple().Equals(other.AsTuple());
		}

		protected abstract ITuple AsTuple();

		public override bool Equals(object? obj)
		{
			return Equals(obj as ValueObject);
		}

		public override int GetHashCode()
		{
			return AsTuple().GetHashCode();
		}

		public static bool operator ==(ValueObject? left, ValueObject? right)
		{
			if (ReferenceEquals(left, right)) return true;

			if (left is null || right is null) return false;

			return left.Equals(right);
		}

		public static bool operator !=(ValueObject? left, ValueObject? right)
		{
			return !(left == right);
		}
	}
}