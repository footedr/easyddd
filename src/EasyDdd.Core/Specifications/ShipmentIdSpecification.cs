﻿using System;
using System.Linq.Expressions;
using EasyDdd.Kernel;

namespace EasyDdd.Core.Specifications
{
	public class ShipmentIdSpecification : Specification<Shipment>
	{
		private readonly string _shipmentId;

		public ShipmentIdSpecification(string shipmentId)
		{
			_shipmentId = shipmentId;
		}

		public override Expression<Func<Shipment, bool>> ToExpression()
		{
			return shipment => shipment.Identifier == _shipmentId;
		}
	}
}