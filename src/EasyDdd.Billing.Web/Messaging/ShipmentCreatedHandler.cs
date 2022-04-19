using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyDdd.Kernel;
using EasyDdd.ShipmentManagement.Core;
using Microsoft.AspNetCore.SignalR;

namespace EasyDdd.Billing.Web.Messaging
{
    public class ShipmentCreatedHandler : ExternalEventHandler<ShipmentCreated>
    {
		private readonly IHubContext<ShipmentsHub> _hubContext;

		public ShipmentCreatedHandler(IHubContext<ShipmentsHub> hubContext)
		{
			_hubContext = hubContext;
		}
		
		public override async Task Handle(ShipmentCreated notification, CancellationToken cancellationToken)
		{
			await _hubContext.PublishMessage(notification.Shipment.Identifier, 
				"shipmentCreated", 
				notification, 
				cancellationToken);
		}
	}
}
