using System;
using UnityEditor;
using UnityEngine;

namespace Events.Editor
{
    public class GameEventViewer : EditorWindow
    {
        private float _sidebarWidth = 200f;
        private Vector2 _scrollPosSidebar = Vector2.zero;
        private Vector2 _scrollPosMainArea = Vector2.zero;

        private GUIContent _recordIcon;
        private GUIContent _notRecordingIcon;

        [MenuItem("Window/General/Game Events")]
        public static void Initialize()
        {
            var window = GetWindow<GameEventViewer>();
            window.titleContent = new GUIContent("Game Events");
            window.Show();
        }

        private void OnEnable()
        {
            _recordIcon = EditorGUIUtility.IconContent("Animation.Record");
            _notRecordingIcon = EditorGUIUtility.IconContent("blendSampler");
        }

        private void OnDisable()
        {
            
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            Sidebar();
            MainArea();
            EditorGUILayout.EndHorizontal();
        }

        private void Sidebar()
        {
            EditorGUILayout.BeginVertical("box", GUILayout.Width(_sidebarWidth));
            _scrollPosSidebar = EditorGUILayout.BeginScrollView(_scrollPosSidebar);
            
            EditorGUILayout.EndScrollView();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();
        }

        private void MainArea()
        {
            EditorGUILayout.BeginVertical();
            SearchBar();
            _scrollPosMainArea = EditorGUILayout.BeginScrollView(_scrollPosMainArea);
            foreach (var recordedEvent in GameEventRecorder.RecordedEvents)
            {
                EventItem(recordedEvent);
            }
            EditorGUILayout.EndScrollView();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();
        }

        private void SearchBar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            if (GUILayout.Button(GameEventRecorder.IsRecording ? _recordIcon : _notRecordingIcon, EditorStyles.toolbarButton, GUILayout.Width(25f)))
            {
                if (GameEventRecorder.IsRecording)
                {
                    GameEventRecorder.StopRecorder();
                }
                else
                {
                    GameEventRecorder.StartRecorder();
                }
            }
            GUILayout.FlexibleSpace();
            GUILayout.Label($"Events: {GameEventRecorder.EventCount}/{GameEventRecorder.MaxEventCount}");
            if (GUILayout.Button("Clear", EditorStyles.toolbarButton))
            {
                GameEventRecorder.ClearCache();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void EventItem(RecordedEvent recordedEvent)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(recordedEvent.DisplayName);
            EditorGUILayout.EndHorizontal();
        }
    }
}
