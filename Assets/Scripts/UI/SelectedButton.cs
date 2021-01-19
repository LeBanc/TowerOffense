using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// SelectedButton class is a specific Button with a selection border
/// The border is shown when the Button is selected (UI navigation) and hide whith Unselect method
/// </summary>
public class SelectedButton : Button
{
    // UI element
    public Image selectedBorder;

    // State of auto-select UI navigation (if true, background is selected when Button is selected)
    public bool navAutoSelect = true;

    // Events
    public delegate void SelectedButtonEventHandler();
    public event SelectedButtonEventHandler OnSelection;

    /// <summary>
    /// OnSelect override is used to call the Select method when the Item is selected via the UI navigation
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        // If auto-select UI navigation is true, select the background
        if(navAutoSelect) Select();
    }

    /// <summary>
    /// Select override shows the selected background image (border) and call the OnSelection event
    /// </summary>
    public override void Select()
    {
        base.Select();
        selectedBorder.enabled = true;
        OnSelection?.Invoke();
    }

    /// <summary>
    /// Unselect method hides the selected background image (border)
    /// </summary>
    public void Unselect()
    {
        selectedBorder.enabled = false;
    }

    /// <summary>
    /// OnDestroy, clears the OnSelection event subscription
    /// </summary>
    protected override void OnDestroy()
    {
        // Clear all event subscription
        onClick.RemoveAllListeners();
        OnSelection = null;
    }
}
