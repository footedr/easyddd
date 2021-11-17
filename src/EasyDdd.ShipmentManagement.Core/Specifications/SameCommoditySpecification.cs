using System;
using System.Linq.Expressions;
using EasyDdd.Kernel;

namespace EasyDdd.ShipmentManagement.Core.Specifications
{
	public class SameCommoditySpecification : Specification<ShipmentDetail>
	{
		private readonly FreightClass _freightClass;
		private readonly PackagingType _packagingType;

		public SameCommoditySpecification(FreightClass freightClass, PackagingType packagingType)
		{
			_freightClass = freightClass;
			_packagingType = packagingType;
		}

		public override Expression<Func<ShipmentDetail, bool>> ToExpression()
		{
			return detail => detail.Class == _freightClass && detail.PackagingType == _packagingType;
		}
	}
}