#define DEEPFREEZE_EVENTS

using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace DeepFreeze.Events
{
    public delegate void EventHandler<in T>(T eventTrigger) where T : class, IEvent;
    
    public static class EventManager
    {
        public static Action<IEvent> OnEventTriggered;
        public static Action<string> OnDebugLog;
        public static Action<string> OnDebugLogError;
        public static Action<Exception> OnDebugLogException;

        private static Dictionary<IEvent, UnityEvent> _events;
        
        private static Dictionary<Type, Delegate> _globalDelegates = new Dictionary<Type, Delegate>();
        private static Dictionary<Type, Dictionary<object, Delegate>> _instancedDelegates = new Dictionary<Type, Dictionary<object, Delegate>>();

        static EventManager()
        {
            OnDebugLog?.Invoke("Initializing");
        }

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

        internal static void LogException(Exception exception)
        {
            OnDebugLogException?.Invoke(exception);
        }
    }
}