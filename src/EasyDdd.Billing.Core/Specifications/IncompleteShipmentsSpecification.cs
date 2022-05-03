using System.Linq.Expressions;
using EasyDdd.Kernel;

namespace EasyDdd.Billing.Core.Specifications;

public class IncompleteShipmentsSpecification : Specification<Shipment>
{
	public override Expression<Func<Shipment, bool>> ToExpression()
	{
		return shipment => shipment.Status != ShipmentStatuses.Delivered && shipment.Status != ShipmentStatuses.Void;
	}
}