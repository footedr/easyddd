using System.Linq.Expressions;
using EasyDdd.Kernel;

namespace EasyDdd.Billing.Core.Specifications;

public class ShipmentByIdSpecification : Specification<Shipment>
{
	private readonly string _shipmentId;

	public ShipmentByIdSpecification(string shipmentId)
	{
		_shipmentId = shipmentId;
	}

	public override Expression<Func<Shipment, bool>> ToExpression()
	{
		return shipment => shipment.Identifier == _shipmentId;
	}
}