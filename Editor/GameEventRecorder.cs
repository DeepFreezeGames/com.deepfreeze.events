﻿using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DeepFreeze.Events.Editor
{
    [InitializeOnLoad]
    public static class GameEventRecorder
    {
        private const int MaxEventCountLimit = 200;
        private const string IsRecordingPrefsKey = "GameEventViewer_IsRecording";
        private const string RecordingMenuTitle = "Window/General/Events/Record Events";
        public static bool IsRecording { get; private set; }

        public static List<RecordedEvent> RecordedEvents { get; private set; } = new();

        public static int EventCount => RecordedEvents.Count;
        public static int MaxEventCount => MaxEventCountLimit;

        static GameEventRecorder()
        {
            IsRecording = EditorPrefs.GetBool(IsRecordingPrefsKey, false);
            Menu.SetChecked(RecordingMenuTitle, IsRecording);
            EditorApplication.playModeStateChanged += HandlePlayModeChange;
        }

        [MenuItem(RecordingMenuTitle)]
        public static void ToggleRecording()
        {
            if (IsRecording)
            {
                StopRecorder();
            }
            else
            {
                StartRecorder();
            }
        }
        
        public static void StartRecorder()
        {
            IsRecording = true;
            EditorPrefs.SetBool(IsRecordingPrefsKey, true);
            Menu.SetChecked(RecordingMenuTitle, IsRecording);
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
            Menu.SetChecked(RecordingMenuTitle, IsRecording);
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
            RecordedEvents.Clear();
            Debug.Log("Cleared message cache");
        }

        private static void LogEvent(IEvent triggeredEvent)
        {
            if (RecordedEvents.Count > MaxEventCount)
            {
                RecordedEvents.RemoveAt(0);
            }
            
            RecordedEvents.Add(new RecordedEvent(triggeredEvent));
        }
    }
}