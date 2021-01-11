using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SaveFileItem : Button
{

    public Image selectedImage;
    public Text saveNameText;
    public Text dateText;
    public Text dayText;

    public delegate void SaveFileItemEventHandler(SaveFileItem _saveFileItem);
    public event SaveFileItemEventHandler OnSelection;

    private string fileName;

    public string FileName
    {
        get { return fileName; }
    }

    public void Setup(string _fileName, DataSave.FileData _fileData)
    {
        fileName = _fileName;
        saveNameText.text = _fileData.SaveName;
        dateText.text = _fileData.Date.ToString("g");
        dayText.text = _fileData.Day.ToString();
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        Select();
    }

    public override void Select()
    {
        base.Select();
        selectedImage.enabled = true;
        OnSelection?.Invoke(this);
    }

    public void Unselect()
    {
        selectedImage.enabled = false;
    }

    protected override void OnDestroy()
    {
        // Clear all event subscription
        OnSelection = null;
    }
}
