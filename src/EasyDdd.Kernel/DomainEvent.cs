using System;
using MediatR;

namespace EasyDdd.Kernel
{
	public record Topic(string Name, string Key);

	public abstract record DomainEvent : INotification
	{
		protected DomainEvent(Topic topic)
		{
			Topic = topic;
			EventId = Guid.NewGuid();
            EventType = GetType().FullName ?? string.Empty;
		}

		public Guid EventId { get; }

		public Topic Topic { get; }

		public string EventType { get; }
	}
}