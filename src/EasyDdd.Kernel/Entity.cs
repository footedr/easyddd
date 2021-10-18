using System;
using System.Collections.Generic;

namespace EasyDdd.Kernel
{
	public abstract class Entity<TId> : IEntity<TId>, IEquatable<Entity<TId>>, IDomainEventSource
	{
		private readonly List<DomainEvent> _domainEvents;

		protected Entity(TId identifier)
		{
			Identifier = identifier;
			_domainEvents = new List<DomainEvent>();
		}

		public virtual IReadOnlyList<DomainEvent> PublishEvents()
		{
			var events = _domainEvents.ToArray();
			_domainEvents.Clear();
			return events;
		}

		public TId Identifier { get; } = default!;

		public bool Equals(Entity<TId>? other)
		{
			if (other is null) return false;

			if (GetType() != other.GetType()) return false;

			if (Identifier == null) return false;

			return Identifier.Equals(other.Identifier);
		}

		public override bool Equals(object? obj)
		{
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return Identifier?.GetHashCode() ?? 0;
		}

		public static bool operator ==(Entity<TId>? left, Entity<TId>? right)
		{
			if (ReferenceEquals(left, right)) return true;

			if (left is null || right is null) return false;

			return left.Equals(right);
		}

		public static bool operator !=(Entity<TId>? left, Entity<TId>? right)
		{
			return !(left == right);
		}

		protected void RecordEvent(DomainEvent domainEvent)
		{
			_domainEvents.Add(domainEvent);
		}
	}
}