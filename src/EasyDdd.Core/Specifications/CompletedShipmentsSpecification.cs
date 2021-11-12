using System;
using System.Linq.Expressions;
using EasyDdd.Kernel;
using NodaTime;

namespace EasyDdd.Core.Specifications;

public class CompletedShipmentsSpecification : Specification<Shipment>
{
	private readonly Instant _from;
	private readonly Instant _to;

	public CompletedShipmentsSpecification(Instant from, Instant to)
	{
		_from = from;
		_to = to;
	}

	public override Expression<Func<Shipment, bool>> ToExpression()
	{
		return s => s.Status == ShipmentStatus.Delivered
					&& s.CreatedAt >= _from && s.CreatedAt <= _to;
	}
}