using UnityEngine;
using UnityEditor;

/// <summary>
/// SoldierNamesDataEditor is the Editor for SoldierNamesData
/// </summary>
[CustomEditor(typeof(SoldierNamesData), true)]
public class SoldierNamesDataEditor : Editor
{
    // SoldierNamesData properties
    SerializedProperty lastNamesFileProperty;
    SerializedProperty maleFirstNamesFileProperty;
    SerializedProperty femaleFirstNamesFileProperty;

    SerializedProperty lastNamesListProperty;
    SerializedProperty maleFirstNamesListProperty;
    SerializedProperty femaleFirstNamesListProperty;

    SoldierNamesData dataTarget;

    /// <summary>
    /// OnEnable, get the properties and the target
    /// </summary>
    protected void OnEnable()
    {
        lastNamesFileProperty = serializedObject.FindProperty("lastNamesFile");
        maleFirstNamesFileProperty = serializedObject.FindProperty("maleFirstNamesFile");
        femaleFirstNamesFileProperty = serializedObject.FindProperty("femaleFirstNamesFile");

        lastNamesListProperty = serializedObject.FindProperty("lastNamesList");
        maleFirstNamesListProperty = serializedObject.FindProperty("maleFirstNamesList");
        femaleFirstNamesListProperty = serializedObject.FindProperty("femaleFirstNamesList");

        dataTarget = (SoldierNamesData)target;
    }

    /// <summary>
    /// OnInspectorGUI, display the properties and buttons to Load/Save the data
    /// </summary>
    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(lastNamesFileProperty);
        EditorGUILayout.PropertyField(maleFirstNamesFileProperty);
        EditorGUILayout.PropertyField(femaleFirstNamesFileProperty);

        if (GUILayout.Button("Load last names"))
        {
            dataTarget.LoadLastNames();
        }
        if (GUILayout.Button("Load male first names"))
        {
            dataTarget.LoadMaleNames();
        }
        if (GUILayout.Button("Load female first names"))
        {
            dataTarget.LoadFemaleNames();
        }

        EditorGUILayout.PropertyField(lastNamesListProperty);
        EditorGUILayout.PropertyField(maleFirstNamesListProperty);
        EditorGUILayout.PropertyField(femaleFirstNamesListProperty);

        if (GUILayout.Button("Save last names"))
        {
            dataTarget.SaveLastNames();
        }
        if (GUILayout.Button("Save male first names"))
        {
            dataTarget.SaveMaleNames();
        }
        if (GUILayout.Button("Save female first names"))
        {
            dataTarget.SaveFemaleNames();
        }

        serializedObject.ApplyModifiedProperties();
    }

}
