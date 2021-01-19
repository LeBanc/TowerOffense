using UnityEngine;
using UnityEditor;

/// <summary>
/// CreateCustomMenu class defines the custom menu options for this project
/// </summary>
public class CreateCustomMenu
{
    /// <summary>
    /// CreateCustomGameObject creates an object from a prefab
    /// </summary>
    /// <param name="gameObjectPath">Prefab path (string)</param>
    /// <param name="menuCommand">Editor command to help parenting (MenuCommand)</param>
    static void CreateCustomGameObject(string gameObjectPath, MenuCommand menuCommand)
    {
        // Create a game object from prefab
        GameObject go = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadMainAssetAtPath(gameObjectPath)) as GameObject;
        // Ensure it gets reparented if this was a context click (otherwise does nothing)
        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
        // Register the creation in the undo system
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        Selection.activeObject = go;
    }

    /// <summary>
    /// All methods below help to create custom UI elements from prefabs
    /// Add a menu item to create custom GameObjects.
    /// Priority 1 ensures it is grouped with the other menu items of the same kind and propagated to the hierarchy dropdown and hierarchy context menus.
    /// </summary>
    /// <param name="menuCommand">Editor command to help parenting (MenuCommand)</param>

    [MenuItem("GameObject/TowerOffense/UI/Text", false, 10)]
    static void CreateUIText(MenuCommand menuCommand)
    {
        CreateCustomGameObject("Assets/Prefabs/UI/_BaseUI/Text.prefab", menuCommand);
    }

    [MenuItem("GameObject/TowerOffense/UI/Button", false, 10)]
    static void CreateUIButton(MenuCommand menuCommand)
    {
        CreateCustomGameObject("Assets/Prefabs/UI/_BaseUI/Button.prefab", menuCommand);
    }

    [MenuItem("GameObject/TowerOffense/UI/Dropdown", false, 10)]
    static void CreateUIDropdown(MenuCommand menuCommand)
    {
        CreateCustomGameObject("Assets/Prefabs/UI/_BaseUI/Dropdown.prefab", menuCommand);
    }

    [MenuItem("GameObject/TowerOffense/UI/Toggle", false, 10)]
    static void CreateUIToggle(MenuCommand menuCommand)
    {
        CreateCustomGameObject("Assets/Prefabs/UI/_BaseUI/Toggle.prefab", menuCommand);
    }

    [MenuItem("GameObject/TowerOffense/UI/Button with Selection border", false, 10)]
    static void CreateUISelectedButton(MenuCommand menuCommand)
    {
        CreateCustomGameObject("Assets/Prefabs/UI/_BaseUI/ButtonWithSelectionBorder.prefab", menuCommand);
    }
}
