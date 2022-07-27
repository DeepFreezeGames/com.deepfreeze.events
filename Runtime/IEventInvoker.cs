using System;
using System.Collections.Generic;

namespace DeepFreeze.Packages.Events.Runtime
{
    public interface IEventInvoker
    {
        object Instance { get; }
        
        IEvent EventObject { get; }

        void Fire(Dictionary<Type, Delegate> delegates);
        
        void Fire(Dictionary<Type, Dictionary<object, Delegate>> delegates);
    }
}