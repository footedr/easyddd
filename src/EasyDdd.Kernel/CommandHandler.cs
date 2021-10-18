using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace EasyDdd.Kernel
{
	public abstract class CommandHandler<TCommand, TResult> : IRequestHandler<TCommand, TResult> where TCommand : Command<TResult>
	{
		public abstract Task<TResult> Handle(TCommand request, CancellationToken cancellationToken);
	}
}