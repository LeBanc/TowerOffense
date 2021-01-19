using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// SaveFileItem class is the item used to display SaveFile (with associated Selecetd Button)
/// SaveFileItem requires the GameObject to have a SelectedButton component
/// </summary>
[RequireComponent(typeof(SelectedButton))]
public class SaveFileItem : MonoBehaviour
{
    // public UI elements
    public Text saveNameText;
    public Text dateText;
    public Text dayText;

    // private associated UI SelectedButton
    private SelectedButton button;

    // Events
    public delegate void SaveFileItemEventHandler(SaveFileItem _saveFileItem);
    public event SaveFileItemEventHandler OnSelection;

    // file name
    private string fileName;

    public string FileName
    {
        get { return fileName; }
    }

    /// <summary>
    /// On Awake, fetches the SelectedButton and subscribe to events
    /// </summary>
    private void Awake()
    {
        button = GetComponent<SelectedButton>();
        if (button != null)
        {
            button.OnSelection += Select;
        }
        else
        {
            Debug.LogError("[SaveFileItem] SelectedButton component not found!");
        }
    }

    /// <summary>
    /// Setup method initialize the item
    /// </summary>
    /// <param name="_fileName">File name / path (string)</param>
    /// <param name="_fileData">File data (FileData)</param>
    public void Setup(string _fileName, DataSave.FileData _fileData)
    {
        fileName = _fileName;
        saveNameText.text = _fileData.SaveName;
        dateText.text = _fileData.Date.ToString("g");
        dayText.text = _fileData.Day.ToString();
    }

    /// <summary>
    /// Select override shows the selected background image (border) and call the OnSelection event
    /// </summary>
    public void Select()
    {
        OnSelection?.Invoke(this);
    }

    /// <summary>
    /// Unselect method hides the selected background image (border)
    /// </summary>
    public void Unselect()
    {
        button.Unselect();
    }

    /// <summary>
    /// OnDestroy, clears the event subscription
    /// </summary>
    protected void OnDestroy()
    {
        // Clear all event subscription
        OnSelection = null;

        if(button != null) button.OnSelection -= Select;
    }
}
