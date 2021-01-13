using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// SoldierImageSelectionItem is a Button displaying a soldier avatar
/// </summary>
public class SoldierImageSelectionItem : Button
{
    // public UI elements
    public Image soldierImage;
    public Mask mask;

    private HQChangeImageCanvas changeImageCanvas;

    /// <summary>
    /// At Awake, disables the mask
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        mask.enabled = false;
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
    public override void Select()
    {
        base.Select();
        mask.enabled = true;
        changeImageCanvas.SelectImage(this);
    }

    /// <summary>
    /// Unselect method deactivates the mask to hide the border
    /// </summary>
    public void Unselect()
    {
        mask.enabled = false;
    }
}
