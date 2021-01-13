using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// NewSaveFileButton class defines a specific Button for New Save
/// </summary>
public class NewSaveFileButton : Button
{
    // UI element
    public Image selectedImage;

    // Event
    public delegate void NewSaveFileButtonEventHandler();
    public event NewSaveFileButtonEventHandler OnSelection;

    /// <summary>
    /// OnSelect override is used to call the Select method when the Item is selected via the UI navigation
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        Select();
    }

    /// <summary>
    /// Select override shows the selected background image (border) and call the OnSelection event
    /// </summary>
    public override void Select()
    {
        base.Select();
        selectedImage.enabled = true;
        OnSelection?.Invoke();
    }

    /// <summary>
    /// Unselect method hides the selected background image (border)
    /// </summary>
    public void Unselect()
    {
        selectedImage.enabled = false;
    }

    /// <summary>
    /// OnDestroy, clears the OnSelection event subscription
    /// </summary>
    protected override void OnDestroy()
    {
        // Clear all event subscription
        OnSelection = null;
        onClick.RemoveAllListeners();
    }
}
