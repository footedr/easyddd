using System;

namespace EasyDdd.Kernel
{
    public class SystemClock : IClock
    {
        public DateTimeOffset Now() => DateTimeOffset.Now;
    }
}
