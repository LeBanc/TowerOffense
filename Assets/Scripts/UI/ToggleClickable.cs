using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// ToggleClickable is a specific Toggle that behave like a button once it is selected
/// With a mouse click will behave like a button
/// It can be selected through UI navigation and the Toggle value will change but the button behaviour will not be activated, it will need a "Submit" key press
/// </summary>
public class ToggleClickable : Toggle
{
    // onClick event
    public delegate void ToggleCLickableEvent();
    public event ToggleCLickableEvent onClick;

    /// <summary>
    /// At Start, subscribe events
    /// </summary>
    protected override void Start()
    {
        base.Start();
        onValueChanged.AddListener(OnValueChanged);
    }

    /// <summary>
    /// OnDestroy, unsubscribe events
    /// </summary>
    protected override void OnDestroy()
    {
        onValueChanged.RemoveAllListeners();
        base.OnDestroy();
    }

    /// <summary>
    /// OnPointerClick, call the onClick event
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        onClick?.Invoke();
    }

    /// <summary>
    /// OnValueChanged changes the state of the ToggleClickable behaviour
    /// </summary>
    /// <param name="_isOn"></param>
    private void OnValueChanged(bool _isOn)
    {
        if (_isOn)
        {
            // If ON, call the SubmitUpdate at PlayUpdate
            GameManager.PlayUpdate += SubmitUpdate;
        }
        else
        {
            // If OFF, doesn't call the SubmitUpdate anymore
            GameManager.PlayUpdate -= SubmitUpdate;
        }
    }

    /// <summary>
    /// SubmitUpdate method listen to Submit input to call onClick event
    /// </summary>
    private void SubmitUpdate()
    {
        if (Input.GetButtonDown("Submit")) onClick?.Invoke();
    }
}


