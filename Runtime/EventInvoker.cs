using System;
using System.Collections.Generic;

namespace Events.Runtime
{
    public class EventInvoker<T> : IEventInvoker where T : class, IEvent
    {
        private readonly T _eventObject;
        public IEvent EventObject => _eventObject;
        
        public object Instance { get; }

        public EventInvoker(T eventObject, object reference)
        {
            _eventObject = eventObject;
            Instance = reference;
        }

        public void Fire(Dictionary<Type, Delegate> delegates)
        {
            var dispatchAs = EventObject.DispatchAs;
            var length = dispatchAs.Length;	
            for (var i = 0; i < length; ++i)
            {
                var key = dispatchAs[i];

                if (!delegates.TryGetValue(key, out var dispatches) || dispatches == null)
                {
                    continue;
                }
                
                try
                {
                    (dispatches as EventHandler<T>)?.Invoke(_eventObject);
                }
                catch (Exception exception)
                {
                    EventManager.LogException(exception);
                }
            }
        }

        public void Fire(Dictionary<Type, Dictionary<object, Delegate>> delegates)
        {
            var dispatchAs = EventObject.DispatchAs;
            var length = EventObject.DispatchAs.Length;
            for (var i = 0; i < length; ++i)
            {
                var key = dispatchAs[i];

                if (!delegates.TryGetValue(key, out var objectPairs) ||
                    !objectPairs.TryGetValue(Instance, out var dispatches) || dispatches == null)
                {
                    continue;
                }
                
                try
                {
                    (dispatches as EventHandler<T>)?.Invoke(_eventObject);
                }
                catch (Exception exception)
                {
                    EventManager.LogException(exception);
                }
            }
        }
    }
}