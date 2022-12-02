using System;
using MediatR;

namespace EasyDdd.Kernel
{
    public abstract record DomainEvent : INotification
    {
        protected DomainEvent()
        {
            EventId = Guid.NewGuid();
        }

        public Guid EventId { get; }

        public abstract AggregateIdentifier GetAggregateIdentifier();
    }
}