using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ColorPickerCanvas is the Canvas of the ColorPicker
/// It sets up the ColorPicker element, Show and Hode the canvas and implements the return buttons
/// </summary>
public class ColorPickerCanvas : CancelableUICanvas
{
    // ColorPicker element
    public ColorPicker colorPicker;
    public Selectable defaultSelectable;

    // Squad currently selected (to update the color of the right squad)
    private Squad selectedSquad;

    private RectTransform colorPickerRect;
    private Vector2 center;
    private Vector2 min;
    private Vector2 max;

    /// <summary>
    /// On Start, fetch the RectTransform and update the rect value
    /// </summary>
    private void Start()
    {
        colorPickerRect = colorPicker.GetComponent<RectTransform>();
        UpdateRectValue();
    }

    /// <summary>
    /// UpdateRectValue update the center, min and max value of the RectTransform for cursor movement when using gamepad
    /// </summary>
    private void UpdateRectValue()
    {
        center = (Vector2)colorPickerRect.position;
        min = center + colorPickerRect.rect.min * CustomInputModule.ScreenRatio;
        max = center + colorPickerRect.rect.max * CustomInputModule.ScreenRatio;
        center = (min + max) / 2;
    }

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
    public override void Show()
    {
        base.Show();

        defaultSelectable.Select();
        UpdateRectValue();
        CursorManager.ShowCursorForAction(center, min, max);

        // Subscribing to PlayUpdate is delayed to avoid multiple UI Selection
        Invoke("SetColorPickerUpdate", Time.deltaTime);
    }

    /// <summary>
    /// SetColorPickerUpdate method subscribe to the PlayUpdate event
    /// </summary>
    private void SetColorPickerUpdate()
    {
        // Link the ColorPickerUpdate to the GameManager.PlayUpdate event
        GameManager.PlayUpdate += colorPicker.ColorPickerUpdate;
    }

    /// <summary>
    /// Hide methods hides the canvas and deactivate the ColorPicker
    /// </summary>
    public override void Hide()
    {
        base.Hide();

        CursorManager.HideCursorAfterAction();

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
    protected override void OnDestroy()
    {
        GameManager.PlayUpdate -= colorPicker.ColorPickerUpdate;
        base.OnDestroy();
    }
}
