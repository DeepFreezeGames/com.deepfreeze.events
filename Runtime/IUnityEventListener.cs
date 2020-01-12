namespace Events.Runtime
{
    public interface IUnityEventListener
    {
        event OnApplicationPausedHandler OnApplicationPaused;

        event OnUpdateHandler OnUpdate;

        event OnLateUpdateHandler OnLateUpdate;

        event OnFixedUpdateHandler OnFixedUpdate;
    }
}