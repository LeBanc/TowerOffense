﻿using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// CancelableUICanvas class is a basic Canvas class for cancelable canvas (with Escape key)
/// </summary>
public class CancelableUICanvas : UICanvas
{
    // private previous selected Selectable
    protected Selectable previousSelected;

    /// <summary>
    /// Show method displays the Canvas, save the previous selected selectable and subscribe to UIManager event
    /// </summary>
    public override void Show()
    {
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            previousSelected = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
        }

        base.Show();
        UIManager.OnHideActiveCanvas += Hide;
    }

    /// <summary>
    /// Hide method hides the Canvas and unsubscribe from events
    /// </summary>
    public override void Hide()
    {
        UIManager.OnHideActiveCanvas -= Hide;
        if (previousSelected != null) previousSelected.Select();
        base.Hide();
    }

    /// <summary>
    /// OnDestroy, unsubscribe from events
    /// </summary>
    protected virtual void OnDestroy()
    {
        UIManager.OnHideActiveCanvas -= Hide;
    }
}
