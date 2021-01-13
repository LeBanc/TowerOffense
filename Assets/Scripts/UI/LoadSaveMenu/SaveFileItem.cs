using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// SaveFileItem class is a Button used to display SaveFile
/// </summary>
public class SaveFileItem : Button
{
    // public UI elements
    public Image selectedImage;
    public Text saveNameText;
    public Text dateText;
    public Text dayText;

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
    /// OnSelect override is used to call the Select method when the Item is selected via the UI navigation
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        Select();
    }

    /// <summary>
    /// Select override shows the selected background image (border) and call the OnSelection event
    /// </summary>
    public override void Select()
    {
        base.Select();
        selectedImage.enabled = true;
        OnSelection?.Invoke(this);
    }

    /// <summary>
    /// Unselect method hides the selected background image (border)
    /// </summary>
    public void Unselect()
    {
        selectedImage.enabled = false;
    }

    /// <summary>
    /// OnDestroy, clears the OnSelection event subscription
    /// </summary>
    protected override void OnDestroy()
    {
        // Clear all event subscription
        OnSelection = null;
    }
}
