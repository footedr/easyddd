using MediatR;

namespace EasyDdd.Kernel;

public abstract record ExternalEvent : INotification
{
}