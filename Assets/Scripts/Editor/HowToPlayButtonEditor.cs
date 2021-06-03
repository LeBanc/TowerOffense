using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(HowToPlayButton), true)]
public class HowToPlayButtonEditor : ButtonEditor
{
    SerializedProperty titleProperty;
    SerializedProperty subtitleProperty;
    SerializedProperty spriteProperty;
    HowToPlayButton howToPlayObject;

    protected override void OnEnable()
    {
        base.OnEnable();
        titleProperty = serializedObject.FindProperty("title");
        subtitleProperty = serializedObject.FindProperty("subtitle");
        spriteProperty = serializedObject.FindProperty("helpImage");
        howToPlayObject = (HowToPlayButton)target;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(titleProperty);
        EditorGUILayout.PropertyField(subtitleProperty);
        EditorGUILayout.PropertyField(spriteProperty);
        serializedObject.ApplyModifiedProperties();
        base.OnInspectorGUI();
        howToPlayObject.UpdateText();        
    }
}
