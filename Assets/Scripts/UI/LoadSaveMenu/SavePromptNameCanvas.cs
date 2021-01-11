using UnityEngine;
using UnityEngine.UI;

public class SavePromptNameCanvas : MonoBehaviour
{
    // public UI elements
    public InputField inputField;
    public Button okButton;
    public Button cancelButton;

    // private canvas component
    private Canvas canvas;

    // Events
    public delegate void SavePromptNameCanvasEventHandler(string _name);
    public event SavePromptNameCanvasEventHandler OnValidate;

    /// <summary>
    /// On Awake, fetches the canvas
    /// </summary>
    private void Awake()
    {
        canvas = GetComponent<Canvas>();
    }

    /// <summary>
    /// Validate method changes the soldier name and hides the canvas
    /// </summary>
    public void Validate()
    {
        OnValidate?.Invoke(inputField.text);
        Hide();
    }

    public void Show()
    {
        inputField.text = "New Save Name";
        canvas.enabled = true;
        transform.SetAsLastSibling();
        inputField.Select();
    }

    /// <summary>
    /// Hide method hides the canvas
    /// </summary>
    public void Hide()
    {
        canvas.enabled = false;
        transform.SetAsFirstSibling();
        OnValidate = null;
    }
}
