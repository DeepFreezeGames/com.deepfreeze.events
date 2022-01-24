using UnityEditor;
using UnityEngine;

namespace Events.Editor
{
    public class GameEventViewer : EditorWindow
    {
        private Vector2 _scrollPosMainArea = Vector2.zero;

        private GUIContent _recordIcon;
        private GUIContent _notRecordingIcon;

        [MenuItem("Window/General/Events/Game Events")]
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
            
            //Update the UI every frame
            EditorApplication.update += Repaint;
        }

        private void OnDisable()
        {
            EditorApplication.update -= Repaint;
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            {
                SearchBar();
                MainArea();
            }
            EditorGUILayout.EndVertical();
        }

        private void MainArea()
        {
            EditorGUILayout.BeginVertical();
            {
                _scrollPosMainArea = EditorGUILayout.BeginScrollView(_scrollPosMainArea);
                {
                    foreach (var recordedEvent in GameEventRecorder.RecordedEvents)
                    {
                        DrawEventItem(recordedEvent);
                    }
                }
                EditorGUILayout.EndScrollView();
                
                GUILayout.FlexibleSpace();
            }
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
            GUILayout.Label($"Events: {GameEventRecorder.EventCount.ToString()}/{GameEventRecorder.MaxEventCount.ToString()}");
            if (GUILayout.Button("Clear", EditorStyles.toolbarButton))
            {
                GameEventRecorder.ClearCache();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawEventItem(RecordedEvent recordedEvent)
        {
            EditorGUILayout.BeginVertical("box");
            {
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Label(recordedEvent.DisplayName, EditorStyles.boldLabel);
                    GUILayout.FlexibleSpace();
                    GUILayout.Label($"{recordedEvent.ExecutionTime}s");
                    if (GUILayout.Button("", GUILayout.Width(24f)))
                    {
                        recordedEvent.expanded = !recordedEvent.expanded;
                    }
                }
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Label(recordedEvent.Type.FullName, EditorStyles.miniLabel);
                    GUILayout.FlexibleSpace();
                }
                EditorGUILayout.EndHorizontal();
                
                if (recordedEvent.expanded)
                {
                    GUILayout.Label("Values", EditorStyles.boldLabel);
                    var width = (position.width / 3f) - 10f;
                    foreach (var propertyValue in recordedEvent.PropertyValues)
                    {
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label(propertyValue.propertyName, GUILayout.Width(width));
                        GUILayout.Label(propertyValue.PropertyType.Name, GUILayout.Width(width));
                        GUILayout.Label(propertyValue.propertyValue, GUILayout.Width(width));
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
            EditorGUILayout.EndVertical();
        }
    }
}
