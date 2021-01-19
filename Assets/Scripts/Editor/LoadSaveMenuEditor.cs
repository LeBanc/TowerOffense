using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LoadSaveMenu), true)]
public class LoadSaveMenuEditor : Editor
{
    SerializedProperty autoScrollProperty;
    SerializedProperty newsavePrefabProperty;
    SerializedProperty promptNameCanvasProperty;
    SerializedProperty savefileitemPrefabProperty;
    SerializedProperty loadStateProperty;

    void OnEnable()
    {
        autoScrollProperty = serializedObject.FindProperty("autoScroll");
        newsavePrefabProperty = serializedObject.FindProperty("newsavePrefab");
        promptNameCanvasProperty = serializedObject.FindProperty("promptNameCanvas");
        savefileitemPrefabProperty = serializedObject.FindProperty("savefileitemPrefab");
        loadStateProperty = serializedObject.FindProperty("loadState");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField(loadStateProperty.boolValue ? "Load config." : "Save config.");

        EditorGUILayout.PropertyField(autoScrollProperty);
        EditorGUILayout.PropertyField(savefileitemPrefabProperty);

        if(!loadStateProperty.boolValue)
        {
            EditorGUILayout.PropertyField(newsavePrefabProperty);
            EditorGUILayout.PropertyField(promptNameCanvasProperty);
        }

        if (GUILayout.Button(loadStateProperty.boolValue ? "Change to save config." : "Change to Load config."))
        {
            loadStateProperty.boolValue = !loadStateProperty.boolValue;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
