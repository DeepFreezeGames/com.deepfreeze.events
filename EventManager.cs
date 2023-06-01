#define DEEPFREEZE_EVENTS
using System;
using System.Collections.Generic;

namespace DeepFreeze.Events
{
    public delegate void EventHandler<in T>(T eventTrigger) where T : class, IEvent;
    
    public static class EventManager
    {
        public static Action<IEvent> OnEventTriggered;

        private static Dictionary<Type, Delegate> _globalDelegates = new Dictionary<Type, Delegate>();
        private static Dictionary<Type, Dictionary<object, Delegate>> _instancedDelegates = new Dictionary<Type, Dictionary<object, Delegate>>();

        public static void ClearAllEvents()
        {
            _globalDelegates.Clear();
            _instancedDelegates.Clear();
        }

        public static Action SubscribeEventListener<T>(EventHandler<T> listener, object instance = null) where T : class, IEvent
        {
            var key = typeof(T);
            if (instance != null)
            {
                AddInstanced(key, listener, instance);
            }
            else
            {
                AddGlobal(key, listener);
            }

            return () => { UnsubscribeEventListener(listener, instance); };
        }

        public static void SubscribeOnce<T>(EventHandler<T> listener, object instance = null) where T : class, IEvent
        {
            void ListenerWrapper(T value)
            {
                UnsubscribeEventListener<T>(ListenerWrapper);
                listener(value);
            }
            
            SubscribeEventListener<T>(ListenerWrapper, instance);
        }

        public static void UnsubscribeEventListener<T>(EventHandler<T> listener, object instance = null) where T : class, IEvent
        {
            var key = typeof(T);
            if (instance != null)
            {
                RemoveInstanced(key, listener, instance);
            }
            else
            {
                RemoveGlobal(key, listener);
            }
        }

        public static void TriggerEvent<T>(T eventTrigger, object instance = null) where T : class, IEvent
        {
            FireEvent(new EventInvoker<T>(eventTrigger, instance));
        }

        private static void FireEvent(IEventInvoker eventInvoker)
        {
            LogEvent(eventInvoker);
            eventInvoker.Fire(_globalDelegates);
            if (eventInvoker.Instance != null)
            {
                eventInvoker.Fire(_instancedDelegates);
            }
        }

        #region GLOBAL DELEGATES
        private static void AddGlobal<T>(Type key, EventHandler<T> listener) where T : class, IEvent
        {
            if (_globalDelegates.ContainsKey(key))
            {
                _globalDelegates[key] = (_globalDelegates[key] as EventHandler<T>) + listener;
            }
            else
            {
                _globalDelegates.Add(key, listener);
            }
        }

        /// <summary>
        /// Returns all of the currently registered listeners for the given <see cref="IEvent"/> type
        /// </summary>
        public static List<Delegate> GetGlobalListeners<T>() where T : class, IEvent
        {
            if (_globalDelegates.TryGetValue(typeof(T), out var del))
            {
                del.
            }

            return null;
        }

        private static void RemoveGlobal<T>(Type key, EventHandler<T> listener) where T : class, IEvent
        {
            if (_globalDelegates.ContainsKey(key))
            {
                _globalDelegates[key] = (_globalDelegates[key] as EventHandler<T>) - listener;
            }
        }
        #endregion

        #region INSTANCED DELEGATES
        private static void AddInstanced<T>(Type key, EventHandler<T> listener, object instance) where T : class, IEvent
        {
            if (!_instancedDelegates.ContainsKey(key))
            {
                _instancedDelegates.Add(key, new Dictionary<object, Delegate>());
            }

            if (_instancedDelegates[key].ContainsKey(instance))
            {
                _instancedDelegates[key][instance] = (_instancedDelegates[key][instance] as EventHandler<T>) + listener;
            }
            else
            {
                _instancedDelegates[key].Add(instance, listener);
            }
        }
        
        

        private static void RemoveInstanced<T>(Type key, EventHandler<T> listener, object instance)
            where T : class, IEvent
        {
            if (_instancedDelegates.ContainsKey(key) && _instancedDelegates[key].ContainsKey(instance))
            {
                _instancedDelegates[key][instance] = (_instancedDelegates[key][instance] as EventHandler<T>) - listener;
            }
        }
        #endregion

        private static void LogEvent(IEventInvoker eventInvoker)
        {
            OnEventTriggered?.Invoke(eventInvoker.EventObject);
        }
    }
}