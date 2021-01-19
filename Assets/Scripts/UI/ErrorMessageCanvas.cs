using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ErrorMessageCanvas is the class managing the Error message Canvas
/// </summary>
public class ErrorMessageCanvas : UICanvas
{
    // Public elements of the Canvas
    public Image errorImage;
    public Text errorText;
    public Button okButton;

    // Default error sprite (from asset)
    public Sprite defaultSprite;

    /// <summary>
    /// On Awake, find the Canvas, suscribe to events and hide the error message
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        Hide();
        okButton.onClick.AddListener(UIManager.HideErrorMessage);
    }

    /// <summary>
    /// OnDestroy, clear all event listeners
    /// </summary>
    private void OnDestroy()
    {
        okButton.onClick.RemoveAllListeners();
    }

    /// <summary>
    /// Show method display the Error Message Canvas on screen with message and optional sprite
    /// </summary>
    /// <param name="_message">Error message to display</param>
    /// <param name="_sprite">Error sprite to display (optional)</param>
    public void Show(string _message, Sprite _sprite = null)
    {
        Show();

        errorText.text = _message;
        if (_sprite != null)
        {
            errorImage.sprite = _sprite;
        }
        else
        {
            errorImage.sprite = defaultSprite;
        }

        okButton.Select();
    }
}
