using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// SoldierImageSelectionItem is a Button displaying a soldier avatar
/// SoldierImageSelectionItem requires the GameObject to have a SelectedButton component
/// </summary>
[RequireComponent(typeof(SelectedButton))]
public class SoldierImageSelectionItem : MonoBehaviour
{
    // public UI elements
    public Image soldierImage;

    // private UI elements
    private SelectedButton button;
    private HQChangeImageCanvas changeImageCanvas;

    /// <summary>
    /// On Start, fetch the SelectedButton, subscribe to event and unselect the item
    /// </summary>
    private void Awake()
    {
        button = GetComponent<SelectedButton>();
        if (button != null)
        {
            button.onClick.AddListener(Select);
        }
        else
        {
            Debug.LogError("[SoldierImageSelectionItem] SelectedButton component not found!");
        }

        Unselect();
    }

    /// <summary>
    /// OnDestroy, unsubscribes from events
    /// </summary>
    void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
        }
    }

    /// <summary>
    /// Setup methods initializes the SoldierImageSelectionItem
    /// </summary>
    /// <param name="_sprite">Avatar to display (Sprite)</param>
    /// <param name="_canvas">Calling Canvas (HQChangeImageCanvas)</param>
    public void Setup(Sprite _sprite, HQChangeImageCanvas _canvas)
    {
        soldierImage.sprite = _sprite;
        changeImageCanvas = _canvas;
    }

    /// <summary>
    /// Select method activates the mask to show the border and set this component as the selected one of the HQChangeImageCanvas
    /// </summary>
    public void Select()
    {
        if (button != null) button.Select();
        if(changeImageCanvas != null) changeImageCanvas.SelectImage(this);
    }

    /// <summary>
    /// Unselect method deactivates the mask to hide the border
    /// </summary>
    public void Unselect()
    {
        button.Unselect();
    }
}
