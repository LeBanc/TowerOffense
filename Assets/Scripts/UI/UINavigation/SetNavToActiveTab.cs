using UnityEngine;
using UnityEngine.UI;
using System.Linq;

/// <summary>
/// SetNavToActiveTab class is used to set the navigation of a button to the active Tab (toggle) of a toggle group
/// SetNavToActiveTab requires the GameObject to have a Button component
/// </summary>
[RequireComponent(typeof(Button))]
public class SetNavToActiveTab : MonoBehaviour
{
    // Targeted toggle group
    public ToggleGroup toggleGroup;

    // public bool to know the nav direction
    public bool navToUp;
    public bool navToDown;
    public bool navToLeft;
    public bool navToRight;


    // Mandatory button component
    private Button button;

    /// <summary>
    /// At Awake, fetches the mandatory button
    /// </summary>
    private void Awake()
    {
        button = GetComponent<Button>();
        if (button == null) Debug.LogError("[SetNavToActiveTab] Mandatory Button not found!");
    }

    /// <summary>
    /// SetNav method gets the active Toggle from the Toggle group and set the navigation to it
    /// It must be call by each toggle when the value change (not the most optimized ...)
    /// </summary>
    /// <param name="_toggleState"></param>
    public void SetNav(bool _toggleState)
    {
        if (!_toggleState) return; // To avoid to set the nav several times at each change

        // Get the active toggle and set the UI navigation of the Button
        Toggle activeToggle = toggleGroup.ActiveToggles().FirstOrDefault();
        if (activeToggle != null)
        {
            Navigation _nav = button.navigation;
            if (navToUp) _nav.selectOnUp = activeToggle;
            if (navToDown) _nav.selectOnDown = activeToggle;
            if (navToLeft) _nav.selectOnLeft = activeToggle;
            if (navToRight) _nav.selectOnRight = activeToggle;
            button.navigation = _nav;
        }
    }
}
