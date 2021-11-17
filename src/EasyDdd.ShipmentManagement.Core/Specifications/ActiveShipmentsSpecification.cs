using System;
using System.Linq.Expressions;
using EasyDdd.Kernel;
using NodaTime;

namespace EasyDdd.ShipmentManagement.Core.Specifications;

public class ActiveShipmentsSpecification : Specification<Shipment>
{
	private readonly Instant _from;
	private readonly Instant _to;

	public ActiveShipmentsSpecification(Instant from, Instant to)
	{
		_from = from;
		_to = to;
	}

	public override Expression<Func<Shipment, bool>> ToExpression()
	{
		return s => (s.Status == ShipmentStatus.Dispatched || s.Status == ShipmentStatus.InTransit)
					&& s.CreatedAt >= _from && s.CreatedAt <= _to;
	}
}