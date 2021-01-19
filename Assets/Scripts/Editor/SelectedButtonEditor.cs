using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(SelectedButton),true)]
public class SelectedButtonEditor : ButtonEditor
{
    SerializedProperty selectedBorderProperty;
    SerializedProperty navAutoSelectProperty;

    protected override void OnEnable()
    {
        base.OnEnable();
        selectedBorderProperty = serializedObject.FindProperty("selectedBorder");
        navAutoSelectProperty = serializedObject.FindProperty("navAutoSelect");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.PropertyField(selectedBorderProperty);
        EditorGUILayout.PropertyField(navAutoSelectProperty);
        serializedObject.ApplyModifiedProperties();
    }
}
