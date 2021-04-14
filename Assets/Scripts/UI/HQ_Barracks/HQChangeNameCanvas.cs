using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// HQChangeNameCanvas class is used by the Change Name Canvas
/// </summary>
public class HQChangeNameCanvas : CancelableUICanvas
{
    // public UI elements
    public InputField inputField;
    public Button okButton;
    public Button cancelButton;

    // private selected soldier
    private Soldier selectedSoldier;
    
    // Events
    public delegate void ChangeNameEventHandler();
    public event ChangeNameEventHandler OnCanvasHide;

    /// <summary>
    /// Validate method changes the soldier name and hides the canvas
    /// </summary>
    public void Validate()
    {
        selectedSoldier.ChangeName(inputField.text);
        Hide();
    }

    /// <summary>
    /// Show method initialize the canvas with a soldier's data
    /// </summary>
    /// <param name="_soldier">Soldier to use</param>
    public void Show(Soldier _soldier)
    {
        selectedSoldier = _soldier;
        inputField.text = selectedSoldier.Name;
        Show();
        inputField.Select();
    }

    /// <summary>
    /// Hide method hides the canvas
    /// </summary>
    public override void Hide()
    {
        base.Hide();
        OnCanvasHide?.Invoke();
    }
}
