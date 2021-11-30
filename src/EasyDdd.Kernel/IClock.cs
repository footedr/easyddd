using System;

namespace EasyDdd.Kernel
{
    public interface IClock
    {
        DateTimeOffset Now();
    }
}
