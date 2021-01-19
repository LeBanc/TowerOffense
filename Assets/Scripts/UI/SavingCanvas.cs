using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SavingCanvas class defines the Saving UI Canvas
/// </summary>
public class SavingCanvas : UICanvas
{
    // public Animation
    public SaveAnimation saveAnimation;

    /// <summary>
    /// Show method shows the canvas and starts the animation
    /// </summary>
    public override void Show()
    {
        base.Show();
        saveAnimation.StartAnimation();
    }

    /// <summary>
    /// Hide method stops the animation and hides the canvas
    /// </summary>
    public override void Hide()
    {
        saveAnimation.StopAnimation();
        base.Hide();
    }

}
