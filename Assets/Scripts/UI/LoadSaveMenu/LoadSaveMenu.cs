using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

/// <summary>
/// LoadSaveMenu class defines the methods of the Load and Save menus
/// </summary>
public class LoadSaveMenu : UICanvas
{
    // UI element
    public AutoScroll autoScroll;
    public GameObject newsavePrefab;
    public SavePromptNameCanvas promptNameCanvas;

    // public bool Load or Save state
    public bool loadState;

    // Prefab for SaveFileItem
    public GameObject savefileitemPrefab;
        
    // Events
    private Dictionary<DataSave.FileData, string> fileDico = new Dictionary<DataSave.FileData, string>();
    private SaveFileItem selectedSaveFile;
    private SelectedButton newSaveFileButton;

    /// <summary>
    /// At Awake, fetches the Canvas and checks if all public elements are well provided
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        if (savefileitemPrefab == null) Debug.LogError("[LoadSaveMenu] Couldn't find SaveFileItemPrefab");
        if (autoScroll == null) Debug.LogError("[LoadSaveMenu] Couldn't find AutoScroll");
        if (!loadState)
        {
            if (newsavePrefab == null) Debug.LogError("[LoadSaveMenu] Couldn't find NewSavePrefab for SaveMenu");
            if (promptNameCanvas == null)
            {
                Debug.LogError("[LoadSaveMenu] Couldn't find PromptName Canvas for SaveMenu");
            }
            else
            {
                promptNameCanvas.OnHide += SelectNewSaveItem;
            }
        }
    }

    /// <summary>
    /// SetPromptNameCanvaseEvent method is used to set or reset the subscribtion to the OnHide event of PromptNameCanvas from outside this class
    /// This is used when exiting the Pause Menu while the PromptNameCanvas is opened to not select the previous
    /// </summary>
    /// <param name="_b">True to subscribe, false to unsubscribe</param>
    public void SetPromptNameCanvaseEvent(bool _b)
    {
        if(_b)
        {
            promptNameCanvas.OnHide -= SelectNewSaveItem;
            promptNameCanvas.OnHide += SelectNewSaveItem;
        }
        else
        {
            promptNameCanvas.OnHide -= SelectNewSaveItem;
        }
    }

    /// <summary>
    /// OnDestroy, unsubscribe from events
    /// </summary>
    private void OnDestroy()
    {
        if(promptNameCanvas != null) promptNameCanvas.OnHide -= SelectNewSaveItem;
    }

    /// <summary>
    /// Show method displays the UI Canvas and setups it depending of load or save state
    /// </summary>
    public override void Show()
    {
        base.Show();
        if(loadState)
        {
            SetupLoadMenu();
        }
        else
        {
            SetupSaveMenu();
        }
    }

    /// <summary>
    /// SetupLoadMenu method setups and shows the Load menu
    /// </summary>
    public void SetupLoadMenu()
    {
        // Clear dictionnary
        fileDico.Clear();

        // Clear AutoScroll
        autoScroll.Clear();

        // Get save files from the directory
        string[] files = Directory.GetFiles(Application.persistentDataPath, "*.save", SearchOption.AllDirectories);
        // Create SaveFileItem from the files
        foreach(string f in files)
        {
            DataSave.FileData _data = DataSave.GetFileData(f);
            if (_data.Date != DateTime.MinValue)
            {
                fileDico.Add(_data, f);
            }
        }
        // Sort the list by DateTime
        List<DataSave.FileData> _fileDataList = fileDico.Keys.ToList();
        _fileDataList.Sort(DataSave.SortByDate);

        // Instantiate SaveFileItem prefab in AutoScroll
        for (int i=0; i<_fileDataList.Count;i++)
        {
            string _fileName;
            fileDico.TryGetValue(_fileDataList[i], out _fileName);
            GameObject _instance = autoScroll.AddPrefabReturnInstance(savefileitemPrefab);
            _instance.GetComponent<SaveFileItem>().Setup(_fileName, _fileDataList[i]);
            _instance.GetComponent<SaveFileItem>().OnSelection += SelectItem;
        }

        // Select first savefile
        autoScroll.SelectFirtsItem();
    }

    /// <summary>
    /// SetupSaveMenu method setups and shows the Save menu
    /// </summary>
    public void SetupSaveMenu()
    {
        // Clear dictionnary
        fileDico.Clear();

        // Clear AutoScroll
        autoScroll.Clear();

        // Instantiate "New Save" element in AutoScroll
        if (newsavePrefab != null)
        {
            GameObject _instance = autoScroll.AddPrefabReturnInstance(newsavePrefab);
            newSaveFileButton = _instance.GetComponent<SelectedButton>();
            newSaveFileButton.OnSelection += UnselectItems;
            newSaveFileButton.onClick.AddListener(PromptNewFileName);
        }

        // Get save files from the directory
        string[] files = Directory.GetFiles(Application.persistentDataPath, "*.save", SearchOption.AllDirectories);
        // Create SaveFileItem from the files
        foreach (string f in files)
        {
            DataSave.FileData _data = DataSave.GetFileData(f);
            if (_data.Date != DateTime.MinValue)
            {
                fileDico.Add(_data, f);
            }
        }
        // Sort the list by DateTime
        List<DataSave.FileData> _fileDataList = fileDico.Keys.ToList();
        _fileDataList.Sort(DataSave.SortByDate);

        // Instantiate SaveFileItem prefab in AutoScroll
        for (int i = 0; i < _fileDataList.Count; i++)
        {
            string _fileName;
            fileDico.TryGetValue(_fileDataList[i], out _fileName);
            GameObject _instance = autoScroll.AddPrefabReturnInstance(savefileitemPrefab);
            _instance.GetComponent<SaveFileItem>().Setup(_fileName, _fileDataList[i]);
            _instance.GetComponent<SaveFileItem>().OnSelection += SelectItem;
        }

        // Select first savefile
        autoScroll.SelectFirtsItem();

        // Hide Prompt Name Canvas
        if (promptNameCanvas != null) promptNameCanvas.Hide();
    }

    /// <summary>
    /// Hide method hides the canvas
    /// </summary>
    public override void Hide()
    {
        base.Hide();
        if (!loadState && promptNameCanvas != null)
        {
            if(promptNameCanvas.IsVisible) promptNameCanvas.Hide();
        }
    }

    /// <summary>
    /// SelectNewSaveItem method selects the NewSave item if the LoadSaveMenu is in Save config.
    /// </summary>
    private void SelectNewSaveItem()
    {
        if(!loadState)
        {
            autoScroll.SelectFirtsItem();
        }
    }

    /// <summary>
    /// SelectItem method set the input item as the selected one
    /// </summary>
    /// <param name="_sfi">Item to select (SaveFileItem)</param>
    private void SelectItem(SaveFileItem _sfi)
    {
        // If same selected item, return
        if (selectedSaveFile == _sfi) return;
        // Else, if an item is selected, unselect it
        if (selectedSaveFile != null) selectedSaveFile.Unselect();
        // Unselect the New save file item if it exists
        if (newSaveFileButton != null) newSaveFileButton.Unselect();
        // Set the new item as the selected one
        selectedSaveFile = _sfi;
    }

    /// <summary>
    /// UnselectItems method unselects the selected item
    /// </summary>
    private void UnselectItems()
    {
        if (selectedSaveFile != null) selectedSaveFile.Unselect();
        selectedSaveFile = null;
    }

    /// <summary>
    /// LoadFile method loads the save file of the selected item
    /// </summary>
    public void LoadFile()
    {
        if (selectedSaveFile != null)
        {
            GameManager.LoadGame(selectedSaveFile.FileName);            
        }
    }

    /// <summary>
    /// PromptNewFileName method shows the Prompt Name Canvas to enter a file name
    /// </summary>
    public void PromptNewFileName()
    {
        if (promptNameCanvas != null)
        {
            promptNameCanvas.OnValidate += CreateNewSaveFile;
            promptNameCanvas.Show();
        }
    }

    /// <summary>
    /// CreateNewSaveFile method creates a save file
    /// </summary>
    /// <param name="_saveName">Name of the save file (string)</param>
    public void CreateNewSaveFile(string _saveName)
    {
        _saveName = Regex.Replace(_saveName, "[/\\:*?\" <>|]", String.Empty);

        // Check for existing file
        if (File.Exists(Path.Combine(Application.persistentDataPath, string.Concat(_saveName, ".save"))))
        {
            // If exist => overwrite message with Save on confirm
            UIManager.InitConfirmMessage("The file already exists, are you sure you want to overwrite it?", delegate {
                StartCoroutine(SaveGame(_saveName));
            });
        }
        else
        {
            // Else Save
            StartCoroutine(SaveGame(_saveName));
        }
    }

    /// <summary>
    /// SaveGame coroutine is the save sequence
    /// </summary>
    /// <param name="_saveName">File name (string)</param>
    /// <returns></returns>
    IEnumerator SaveGame(string _saveName)
    {
        float startTime = Time.time;
        // Change the GameState to display the save animation
        GameManager.ChangeGameStateRequest(GameManager.GameState.save);
        new DataSave().SaveGame(_saveName);
        while(Time.time < startTime +1f)
        {
            yield return null;
        }
        // Return to the Pause GameState
        GameManager.ChangeGameStateRequest(GameManager.GameState.pause);
    }

    /// <summary>
    /// SaveFile method is called by the "Save" button
    /// </summary>
    public void SaveFile()
    {
        // Overwrite message with Save on confirm
        if (selectedSaveFile != null)
        {
            UIManager.InitConfirmMessage("The file already exists, are you sure you want to overwrite it?", delegate { StartCoroutine(SaveGame(selectedSaveFile.saveNameText.text)); });
        }
        else // no file selected = create new save
        {
            PromptNewFileName();
        }
    }
    
    /// <summary>
    /// DeleteFile method is called by the "Delete" button
    /// </summary>
    public void DeleteFile()
    {
        // Do nothing if no save file is selected
        if (selectedSaveFile == null) return;

        // Confirm message with delete on confirm
        UIManager.InitConfirmMessage("Are you sure you want to delete this save file?", delegate {
            if (File.Exists(Path.Combine(Application.persistentDataPath, selectedSaveFile.FileName)))
            {
                // Delete the file
                File.Delete(selectedSaveFile.FileName);
                // Remove events from the element
                selectedSaveFile.OnSelection -= SelectItem;
                // And remove the object from the autoscroll
                autoScroll.Remove(selectedSaveFile.gameObject);
            }
        });
    }

    /// <summary>
    /// Cancel method is called by the "Cancel" button
    /// It hides the currently opened Menu (load or save) through the UIManager static methods
    /// </summary>
    public void Cancel()
    {
        if(loadState)
        {
            UIManager.HideLoadMenu();
        }
        else
        {
            UIManager.HideSaveMenu();
        }

        if (IsVisible)
        {
            Debug.Log("LoadSaveMenu still visible after calling Hide methods from UIManager: Hiding itself");
            Hide();
        }
    }

    /// <summary>
    /// GetLastSavedFile method is a static method that returns the name of the last saved file
    /// </summary>
    /// <returns>Name of the last saved file or empty string if no save file is found (string)</returns>
    public static string GetLastSavedFile()
    {
        Dictionary<DataSave.FileData, string> _dico = new Dictionary<DataSave.FileData, string>();

        // Get save files from the directory
        string[] files = Directory.GetFiles(Application.persistentDataPath, "*.save", SearchOption.AllDirectories);
        // Create SaveFileItem from the files
        foreach (string f in files)
        {
            DataSave.FileData _data = DataSave.GetFileData(f);
            if (_data.Date != DateTime.MinValue)
            {
                _dico.Add(_data, f);
            }
        }

        // If dictionnary is empty, return now the default value
        if (_dico.Count == 0) return "";

        // Sort the list by DateTime
        List<DataSave.FileData> _fileDataList = _dico.Keys.ToList();
        _fileDataList.Sort(DataSave.SortByDate);
        string _fileName = "";
        _dico.TryGetValue(_fileDataList[0], out _fileName);

        return _fileName;
    }
}
