using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// AutoToggle class is a Toggle that automatically switches on when selected
/// </summary>
public class AutoToggle : Toggle
{
    /// <summary>
    /// OnSelect override set the Toggle as ON when it is selected via UI navigation
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        isOn = true;
    }
}
