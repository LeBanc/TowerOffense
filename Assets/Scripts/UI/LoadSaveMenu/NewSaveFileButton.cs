using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NewSaveFileButton : Button
{
    public Image selectedImage;

    public delegate void NewSaveFileButtonEventHandler();
    public event NewSaveFileButtonEventHandler OnSelection;

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        Select();
    }

    public override void Select()
    {
        base.Select();
        selectedImage.enabled = true;
        OnSelection?.Invoke();
    }

    public void Unselect()
    {
        selectedImage.enabled = false;
    }

    protected override void OnDestroy()
    {
        // Clear all event subscription
        OnSelection = null;
        onClick.RemoveAllListeners();
    }
}
