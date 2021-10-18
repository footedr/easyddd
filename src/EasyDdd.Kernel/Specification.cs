using System;
using System.Linq.Expressions;

namespace EasyDdd.Kernel
{
	public abstract class Specification<T>
	{
		public abstract Expression<Func<T, bool>> ToExpression();
	}
}