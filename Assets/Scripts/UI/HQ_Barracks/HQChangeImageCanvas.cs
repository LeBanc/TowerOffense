using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// HQChangeImageCanvas class is the class of the Change Image Canvas
/// </summary>
public class HQChangeImageCanvas : UICanvas
{
    // public UI elements
    public AutoScroll autoScroll;
    public GameObject soldierImageItem;

    // List of available soldier sprites
    private List<SoldierImageSelectionItem> soldierImageItemList = new List<SoldierImageSelectionItem>();

    // Selected soldier and image
    private Soldier selectedSoldier;
    private SoldierImageSelectionItem selectedImageItem;

    // Events
    public delegate void ChangeImageEventHandler();
    public event ChangeImageEventHandler OnCanvasHide;

    /// <summary>
    /// On Start, initializes the available sprites list
    /// </summary>
    private void Start()
    {
        foreach (Sprite _sprite in PlayManager.data.soldierImages)
        {
            GameObject _go = autoScroll.AddPrefabReturnInstance(soldierImageItem);
            SoldierImageSelectionItem _item = _go.GetComponent<SoldierImageSelectionItem>();
            _item.Setup(_sprite, this);
            soldierImageItemList.Add(_item);
        }
    }

    /// <summary>
    /// Show method displays the canvas
    /// </summary>
    /// <param name="_soldier"></param>
    public void Show(Soldier _soldier = null)
    {
        Show();

        if(_soldier != null)
        {
            selectedSoldier = _soldier;
            selectedImageItem = soldierImageItemList.Find(item => item.soldierImage.sprite == _soldier.Image);
            if (selectedImageItem != null)
            {
                selectedImageItem.Select();
            }
            else
            {
                autoScroll.SelectFirtsItem();
            }
        }
    }

    /// <summary>
    /// SelectImage method changes the image selection
    /// </summary>
    /// <param name="_item">Image to select</param>
    public void SelectImage(SoldierImageSelectionItem _item)
    {
        if(_item != selectedImageItem)
        {
            selectedImageItem.Unselect();
            selectedImageItem = _item;
        }
    }

    /// <summary>
    /// Hide method hides the canvas
    /// </summary>
    public override void Hide()
    {
        if(selectedImageItem != null) selectedImageItem.Unselect();
        selectedSoldier = null;

        base.Hide();
        OnCanvasHide?.Invoke();
    }

    /// <summary>
    /// Validate method changes the soldier image and hides the canvas
    /// </summary>
    public void Validate()
    {
        selectedSoldier.ChangeImage(selectedImageItem.soldierImage.sprite);
        Hide();
    }

}
