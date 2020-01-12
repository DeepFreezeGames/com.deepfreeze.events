using System;
using UnityEngine;

namespace Events.Runtime
{
    public delegate void OnApplicationPausedHandler(bool pauseStatus);
    public delegate void OnUpdateHandler(float deltaTime, float unscaledDeltaTime);
    public delegate void OnLateUpdateHandler(float deltaTime, float unscaledDeltaTime);
    public delegate void OnFixedUpdateHandler(float fixedUpdate);
    
    public class UnityEventListener : MonoBehaviour, IUnityEventListener
    {
        public event OnApplicationPausedHandler OnApplicationPaused;
        public event OnUpdateHandler OnUpdate;
        public event OnLateUpdateHandler OnLateUpdate;
        public event OnFixedUpdateHandler OnFixedUpdate;

        protected void OnApplicationPause(bool pauseStatus)
        {
            OnApplicationPaused?.Invoke(pauseStatus);
        }
 
        protected void Update()
        {
            OnUpdate?.Invoke(Time.deltaTime, Time.unscaledDeltaTime);
        }

        protected void LateUpdate()
        {
            OnLateUpdate?.Invoke(Time.deltaTime, Time.unscaledDeltaTime);
        }

        protected void FixedUpdate()
        {
            OnFixedUpdate?.Invoke(Time.fixedDeltaTime);
        }

        public void OnDestroy()
        {
            OnApplicationPaused = null;
            OnUpdate = null;
            OnLateUpdate = null;
            OnFixedUpdate = null;
        }
    }
}