using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyDdd.Kernel;

namespace EasyDdd.Core
{
    public class TrackingEvent : Entity<string>
    {
		public TrackingEvent()
			: base(Guid.NewGuid().ToString())
		{

		}
    }
}
