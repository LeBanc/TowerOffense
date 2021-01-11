using UnityEngine;

/// <summary>
/// ColorPickerCanvas is the Canvas of the ColorPicker
/// It sets up the ColorPicker element, Show and Hode the canvas and implements the return buttons
/// </summary>
public class ColorPickerCanvas : MonoBehaviour
{
    // ColorPicker element
    public ColorPicker colorPicker;

    // Squad currently selected (to update the color of the right squad)
    private Squad selectedSquad;

    /// <summary>
    /// Setup method sets the selected squad and sets up the ColorPicker
    /// </summary>
    /// <param name="_squad">Selected squad</param>
    public void Setup(Squad _squad)
    {
        selectedSquad = _squad;
        colorPicker.Setup(_squad.Color);
    }

    /// <summary>
    /// Show methods shows the canvas on screen and activate the ColorPicker
    /// </summary>
    public void Show()
    {
        // Enable the Canvas and sets it at last sibling to be sure it is visible
        GetComponent<Canvas>().enabled = true;
        GetComponent<Transform>().SetAsLastSibling();
        // Link the ColorPickerUpdate to the GameManager.PlayUpdate event
        GameManager.PlayUpdate += colorPicker.ColorPickerUpdate;
    }

    /// <summary>
    /// Hide methods hides the canvas and deactivate the ColorPicker
    /// </summary>
    public void Hide()
    {
        // Disable the Canvas and sets it at first sibling to be sure it is not visible or over other canvas
        GetComponent<Canvas>().enabled = false;
        GetComponent<Transform>().SetAsFirstSibling();
        // Unlink the ColorPickerUpdate from the GameManager.PlayUpdate event
        GameManager.PlayUpdate -= colorPicker.ColorPickerUpdate;
    }

    /// <summary>
    /// Cancel method has to be called by the cancel button, it simply hides the canvas
    /// </summary>
    public void Cancel()
    {
        Hide();
    }

    /// <summary>
    /// Validate method has to be called by the OK button. It sets the selected color to the selected squad and hides the Canvas.
    /// </summary>
    public void Validate()
    {
        selectedSquad.ChangeColor(colorPicker.GetColor());
        Hide();
    }

    /// <summary>
    /// OnDestroy methods unliks the may be still linked event listeners
    /// </summary>
    private void OnDestroy()
    {
        GameManager.PlayUpdate -= colorPicker.ColorPickerUpdate;
    }

}
