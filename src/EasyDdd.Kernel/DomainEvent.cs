using System;
using MediatR;

namespace EasyDdd.Kernel
{
	public record Topic(string Name, string Key);

	public abstract record DomainEvent : INotification
	{
		protected DomainEvent()
		{
			EventId = Guid.NewGuid();
		}

		protected DomainEvent(string topicName, string topicKey)
			: this()
		{
			Topic = new Topic(topicName, topicKey);
		}

		public Guid EventId { get; }
		public Topic? Topic { get; private set; }
	}
}