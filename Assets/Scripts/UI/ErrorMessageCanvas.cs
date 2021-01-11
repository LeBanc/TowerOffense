using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ErrorMessageCanvas is the class managing the Error meassage Canvas
/// </summary>
public class ErrorMessageCanvas : MonoBehaviour
{
    // Public elements of the Canvas
    public Image errorImage;
    public Text errorText;
    public Button okButton;

    // Default error sprite (from asset)
    public Sprite defaultSprite;

    // Attached Canvas
    private Canvas canvas;

    public bool IsShown
    {
        get { return canvas.enabled; }
    }

    /// <summary>
    /// On Awake, find the Canvas, suscribe to events and hide the error message
    /// </summary>
    private void Awake()
    {
        canvas = GetComponent<Canvas>();
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
    /// Show method display the Error Canvas on screen with message and optional sprite
    /// </summary>
    /// <param name="_message">Error message to display</param>
    /// <param name="_sprite">Error sprite to display (optional)</param>
    public void Show(string _message, Sprite _sprite = null)
    {
        canvas.enabled = true;
        transform.SetAsLastSibling();
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

    /// <summary>
    /// Hide method hides the Error Canvas
    /// </summary>
    public void Hide()
    {
        transform.SetAsFirstSibling(); 
        canvas.enabled = false;
    }
}
