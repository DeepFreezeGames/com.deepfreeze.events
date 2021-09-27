using System;
using System.Collections.Generic;
using UnityEngine;

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
            if (!delegates.TryGetValue(dispatchAs, out var dispatches) || dispatches == null)
            {
                return;
            }
                
            try
            {
                (dispatches as EventHandler<T>)?.Invoke(_eventObject);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                EventManager.LogException(exception);
            }
        }

        public void Fire(Dictionary<Type, Dictionary<object, Delegate>> delegates)
        {
            if (!delegates.TryGetValue(EventObject.DispatchAs, out var objectPairs) ||
                !objectPairs.TryGetValue(Instance, out var dispatches) || dispatches == null)
            {
                return;
            }
                
            try
            {
                (dispatches as EventHandler<T>)?.Invoke(_eventObject);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                EventManager.LogException(exception);
            }
        }
    }
}
