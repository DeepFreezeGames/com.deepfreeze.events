﻿using System;
using System.Collections.Generic;
using Events.Runtime;
using UnityEditor;
using UnityEngine;

namespace Events.Editor
{
    [InitializeOnLoad]
    public static class GameEventRecorder
    {
        private const int MaxEventCountLimit = 200;
        private const string IsRecordingPrefsKey = "GameEventViewer_IsRecording";
        public static bool IsRecording { get; private set; }

        private static readonly List<RecordedEvent> _recordedEvents;
        public static List<RecordedEvent> RecordedEvents => _recordedEvents;

        public static int EventCount => RecordedEvents.Count;
        public static int MaxEventCount => MaxEventCountLimit;

        static GameEventRecorder()
        {
            IsRecording = EditorPrefs.GetBool(IsRecordingPrefsKey, true);
            _recordedEvents = new List<RecordedEvent>();
            EditorApplication.playModeStateChanged += HandlePlayModeChange;
        }
        
        public static void StartRecorder()
        {
            IsRecording = true;
            EditorPrefs.SetBool(IsRecordingPrefsKey, true);
            if (EditorApplication.isPlaying)
            {
                StartRecording();
            }
        }

        private static void StartRecording()
        {
            EventManager.OnEventTriggered += LogEvent;
            Debug.Log("Started recording");
        }

        public static void StopRecorder()
        {
            IsRecording = false;
            EditorPrefs.SetBool(IsRecordingPrefsKey, false);
            if (EditorApplication.isPlaying)
            {
                StopRecording();
            }
        }

        private static void StopRecording()
        {
            EventManager.OnEventTriggered -= LogEvent;
            Debug.Log("Stopped recording");
        }

        private static void HandlePlayModeChange(PlayModeStateChange playModeStateChange)
        {
            switch (playModeStateChange)
            {
                case PlayModeStateChange.EnteredEditMode:
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    if (IsRecording)
                    {
                        StartRecording();
                    }
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    if (IsRecording)
                    {
                        StopRecording();
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(playModeStateChange), playModeStateChange, null);
            }
        }

        public static void ClearCache()
        {
            _recordedEvents.Clear();
            Debug.Log("Cleared message cache");
        }

        private static void LogEvent(IEvent triggeredEvent)
        {
            if (_recordedEvents.Count > MaxEventCount)
            {
                _recordedEvents.RemoveAt(0);
            }
            
            _recordedEvents.Add(new RecordedEvent(triggeredEvent));
        }
    }
}