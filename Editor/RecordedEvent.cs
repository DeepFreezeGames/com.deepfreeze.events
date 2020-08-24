﻿using System;
using System.Collections.Generic;
using Events.Runtime;
using UnityEngine;

namespace Events.Editor
{
    [Serializable]
    public class RecordedEvent
    {
        public IEvent Event { get; }
        public Type Type { get; }
        public string DisplayName { get; }
        public string ExecutionTime { get; }
        public List<PropertyValue> PropertyValues { get; }
    
        public bool expanded;
    
        public RecordedEvent(IEvent triggeredEvent)
        {
            Event = triggeredEvent;
            Type = triggeredEvent.GetType();
            DisplayName = triggeredEvent.GetType().Name;
            ExecutionTime = Time.time.ToString("F");
    
            PropertyValues = new List<PropertyValue>();
            var properties = Type.GetProperties();
            foreach (var property in properties)
            {
                PropertyValues.Add(new PropertyValue(property.Name, property.GetValue(triggeredEvent).ToString()));
            }
        }
        
        [Serializable]
        public class PropertyValue
        {
            public string propertyName;
            public string propertyValue;
    
            private PropertyValue() { }
    
            public PropertyValue(string name, string value)
            {
                propertyName = name;
                propertyValue = value;
            }
        }
    }
}