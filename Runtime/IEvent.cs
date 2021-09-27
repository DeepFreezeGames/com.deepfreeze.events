using System;

namespace Events.Runtime
{
    public interface IEvent
    {
        Type DispatchAs { get; }
    }
}