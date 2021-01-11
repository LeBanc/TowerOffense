using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SoldierImageSelectionItem : Button
{
    public Image soldierImage;
    public Mask mask;

    private HQChangeImageCanvas changeImageCanvas;

    protected override void Awake()
    {
        base.Awake();
        mask.enabled = false;
    }

    public void Setup(Sprite _sprite, HQChangeImageCanvas _canvas)
    {
        soldierImage.sprite = _sprite;
        changeImageCanvas = _canvas;
    }

    public override void Select()
    {
        base.Select();
        mask.enabled = true;
        changeImageCanvas.SelectImage(this);
    }

    public void Unselect()
    {
        mask.enabled = false;
    }
}
