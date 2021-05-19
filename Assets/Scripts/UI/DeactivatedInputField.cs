using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DeactivatedInputField : InputField
{
    /// <summary>
    /// OnSelect method only set the InputField as Selected (no Activation for Joystick Controllers handling)
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnSelect(BaseEventData eventData)
    {
        DoStateTransition(SelectionState.Selected, true);
    }
}
