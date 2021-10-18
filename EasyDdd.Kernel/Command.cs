using MediatR;

namespace EasyDdd.Kernel
{
	public abstract record Command<TResult> : IRequest<TResult>
	{
	}
}