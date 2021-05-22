using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ControllerImageSwitcher class defines the switching behaviour of images for controller change
/// </summary>
public class ControllerImageSwitcher : MonoBehaviour
{
    public Image keyboardImage;
    public Image gamepadImage;

    /// <summary>
    /// At Start, subscribe to events
    /// </summary>
    void Start()
    {
        CursorManager.OnMouseControlChange += ChangeToKeyboard;
        CursorManager.OnGamePadeControlChange += ChangeToGamepad;
    }

    /// <summary>
    /// OnDestroy, unsubscribe from events
    /// </summary>
    private void OnDestroy()
    {
        CursorManager.OnMouseControlChange -= ChangeToKeyboard;
        CursorManager.OnGamePadeControlChange -= ChangeToGamepad;
    }

    /// <summary>
    /// ChangeToKeyboard method changes the image's sprite to the keyboard sprite
    /// </summary>
    private void ChangeToKeyboard()
    {
        // Show the keyboard image and hide the gamepad one
        if (keyboardImage != null) keyboardImage.gameObject.SetActive(true);
        if (gamepadImage != null) gamepadImage.gameObject.SetActive(false);
    }

    // <summary>
    /// ChangeToGamepad method changes the image's sprite to the gamepad sprite
    /// </summary>
    private void ChangeToGamepad()
    {
        // Show the gamepad image and hide the keyboard one
        if (gamepadImage != null) gamepadImage.gameObject.SetActive(true);
        if (keyboardImage != null) keyboardImage.gameObject.SetActive(false);
    }
}
