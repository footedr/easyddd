using System;
using System.Linq.Expressions;
using EasyDdd.Kernel;

namespace EasyDdd.ShipmentManagement.Core.Specifications
{
	public class ShipmentIdSpecification : Specification<Shipment>
	{
		private readonly ShipmentId _shipmentId;

		public ShipmentIdSpecification(ShipmentId shipmentId)
		{
			_shipmentId = shipmentId;
		}

		public override Expression<Func<Shipment, bool>> ToExpression()
		{
			return shipment => shipment.Identifier == _shipmentId;
		}
	}
}