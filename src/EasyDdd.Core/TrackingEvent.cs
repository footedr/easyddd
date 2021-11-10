using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodaTime;

namespace EasyDdd.Core
{
    public record TrackingEvent(TrackingEventType Type, LocalDateTime Occurred, Instant Created, string CreatedBy)
    {
    }
}
