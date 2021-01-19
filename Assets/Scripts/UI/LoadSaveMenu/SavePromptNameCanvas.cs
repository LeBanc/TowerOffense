using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// SavePromptNameCanvas defines the Canvas prompted when asking a Save File Name
/// </summary>
public class SavePromptNameCanvas : UICanvas
{
    // public UI elements
    public InputField inputField;
    public Button okButton;
    public Button cancelButton;

    // Events
    public delegate void SavePromptNameCanvasEventHandler(string _name);
    public event SavePromptNameCanvasEventHandler OnValidate;

    /// <summary>
    /// Validate method changes the soldier name and hides the canvas
    /// </summary>
    public void Validate()
    {
        OnValidate?.Invoke(inputField.text);
        Hide();
    }

    /// <summary>
    /// Show method inits and displays the SavePromptNameCanvas
    /// </summary>
    public override void Show()
    {
        inputField.text = "New Save Name";
        base.Show();
        inputField.Select();
    }

    /// <summary>
    /// Hide method hides the canvas
    /// </summary>
    public override void Hide()
    {
        base.Hide();
        OnValidate = null;
    }
}
