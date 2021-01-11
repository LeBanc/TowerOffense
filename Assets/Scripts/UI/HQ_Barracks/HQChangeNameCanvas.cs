using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// HQChangeNameCanvas class is used by the Change Name Canvas
/// </summary>
public class HQChangeNameCanvas : MonoBehaviour
{
    // public UI elements
    public InputField inputField;
    public Button okButton;
    public Button cancelButton;

    // private canvas component
    private Canvas canvas;

    // private selected soldier
    private Soldier selectedSoldier;
    
    // Events
    public delegate void ChangeNameEventHandler();
    public event ChangeNameEventHandler OnCanvasHide;

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
        OnCanvasHide?.Invoke();
    }
}
