using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToggleClickable : Toggle
{
    public delegate void ToggleCLickableEvent();
    public event ToggleCLickableEvent onClick;

    protected override void Start()
    {
        base.Start();
        onValueChanged.AddListener(OnValueChanged);
    }

    protected override void OnDestroy()
    {
        onValueChanged.RemoveAllListeners();
        base.OnDestroy();
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        onClick?.Invoke();
    }

    private void OnValueChanged(bool _isOn)
    {
        if (_isOn)
        {
            GameManager.PlayUpdate += SubmitUpdate;
        }
        else
        {
            GameManager.PlayUpdate -= SubmitUpdate;
        }
    }

    private void SubmitUpdate()
    {
        if (Input.GetButtonDown("Submit")) onClick?.Invoke();
    }
}


