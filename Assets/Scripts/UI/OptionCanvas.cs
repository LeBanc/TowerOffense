using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// OptionCanvas class defines the Options Canvas
/// </summary>
public class OptionCanvas : CancelableUICanvas
{
    // TabToggle
    public ToggleGroup tabs;

    /// <summary>
    /// Show method shows the canvas and subscribes to events
    /// </summary>
    public override void Show()
    {
        GameManager.StartUpdate += OptionCanvasUpdate;
        GameManager.PauseUpdate += OptionCanvasUpdate;
        base.Show();
    }

    /// <summary>
    /// Hide method hides the canavs and unsubscribes from events
    /// </summary>
    public override void Hide()
    {
        base.Hide();
        GameManager.StartUpdate -= OptionCanvasUpdate;
        GameManager.PauseUpdate -= OptionCanvasUpdate;
    }

    /// <summary>
    /// OnDestroy, unsubscribes from events
    /// </summary>
    protected override void OnDestroy()
    {
        GameManager.StartUpdate -= OptionCanvasUpdate;
        GameManager.PauseUpdate -= OptionCanvasUpdate;
        base.OnDestroy();
    }

    /// <summary>
    /// OptionCanvasUpdate method is the update method of the Option Canvas used to switch between tabs (toggle)
    /// </summary>
    private void OptionCanvasUpdate()
    {
        // Check Inputs for right or left selection buttons => Change tabs
        if (Input.GetButtonDown("RightSelection"))
        {
            List<Toggle> toggleList = tabs.ActiveToggles().ToList();
            if(toggleList[0].navigation.selectOnRight != null)
            {
                if (toggleList[0].navigation.selectOnRight.TryGetComponent(out Toggle _t))
                {
                    _t.Select();
                }
            }
        }
        else if (Input.GetButtonDown("LeftSelection"))
        {
            List<Toggle> toggleList = tabs.ActiveToggles().ToList();
            if (toggleList[0].navigation.selectOnLeft != null)
            {
                if (toggleList[0].navigation.selectOnLeft.TryGetComponent(out Toggle _t))
                {
                    _t.Select();
                }
            }
        }
    }
}
