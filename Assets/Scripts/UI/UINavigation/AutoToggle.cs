using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;
using System.Collections;

public class AutoToggle : Toggle
{
    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        isOn = true;
    }
}
