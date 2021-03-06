﻿using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// ConfirmMessageCanvas is the class managing the Confirm message Canvas
/// </summary>
public class ConfirmMessageCanvas : UICanvas
{
    // Public elements of the Canvas
    public Image errorImage;
    public Text errorText;
    public Button okButton;
    public Button cancelButton;

    // Default error sprite (from asset)
    public Sprite defaultSprite;

    /// <summary>
    /// On Awake, find the Canvas, suscribe to events and hide the confirm message
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        Hide();
        cancelButton.onClick.AddListener(UIManager.HideConfirmMessage);
    }

    /// <summary>
    /// OnDestroy, clear all event listeners
    /// </summary>
    private void OnDestroy()
    {
        cancelButton.onClick.RemoveAllListeners();
        okButton.onClick.RemoveAllListeners();
    }

    /// <summary>
    /// Show method display the Confirm Message Canvas on screen with message and optional sprite
    /// </summary>
    /// <param name="_message">Error message to display</param>
    /// <param name="_callback">Callback method to call when clicking OK</param>
    /// <param name="_sprite">Error sprite to display (optional)</param>
    public void Show(string _message, Action _callback, Sprite _sprite = null)
    {
        Show();

        errorText.text = _message;
        if (_sprite != null)
        {
            errorImage.sprite = _sprite;
        }
        else
        {
            errorImage.sprite = defaultSprite;
        }

        okButton.onClick.AddListener(Hide);
        okButton.onClick.AddListener(delegate { _callback(); });

        cancelButton.Select();
    }

    /// <summary>
    /// Hide method hides the Confirm Message Canvas
    /// </summary>
    public override void Hide()
    {
        base.Hide();
        okButton.onClick.RemoveAllListeners();
    }

}
