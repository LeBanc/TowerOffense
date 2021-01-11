using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// HQChangeImageCanvas class is the class of the Change Image Canvas
/// </summary>
public class HQChangeImageCanvas : MonoBehaviour
{
    // public UI elements
    public AutoScroll autoScroll;
    public GameObject soldierImageItem;

    // private own Canvas
    private Canvas canvas;

    // List of available soldier sprites
    private List<SoldierImageSelectionItem> soldierImageItemList = new List<SoldierImageSelectionItem>();

    // Selected soldier and image
    private Soldier selectedSoldier;
    private SoldierImageSelectionItem selectedImageItem;

    // Events
    public delegate void ChangeImageEventHandler();
    public event ChangeImageEventHandler OnCanvasHide;

    /// <summary>
    /// On Awake, fetches the canvas
    /// </summary>
    private void Awake()
    {
        canvas = GetComponent<Canvas>();
    }

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
        canvas.enabled = true;
        transform.SetAsLastSibling();

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
    public void Hide()
    {
        if(selectedImageItem != null) selectedImageItem.Unselect();
        selectedSoldier = null;

        transform.SetAsFirstSibling();
        canvas.enabled = false;
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
