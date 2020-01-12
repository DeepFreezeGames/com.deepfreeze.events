using System;
using Events.Runtime;
using UnityEngine;

namespace Events.Editor
{
    [Serializable]
    public class RecordedEvent
    {
        public IEvent Event { get; }
        public string DisplayName { get; }

        public RecordedEvent(IEvent triggeredEvent)
        {
            Event = triggeredEvent;
            DisplayName = $"{Time.time} {triggeredEvent.GetType().Name}";
        }
    }
}