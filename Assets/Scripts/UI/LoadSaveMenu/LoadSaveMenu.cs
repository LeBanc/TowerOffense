using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

/// <summary>
/// LoadSaveMenu class defines the methods of the Load and Save menus
/// </summary>
public class LoadSaveMenu : MonoBehaviour
{
    // UI element
    public AutoScroll autoScroll;
    public GameObject newsavePrefab;
    public SavePromptNameCanvas promptNameCanvas;

    // Prefab for SaveFileItem
    public GameObject savefileitemPrefab;
        
    // Events
    private Dictionary<DataSave.FileData, string> fileDico = new Dictionary<DataSave.FileData, string>();
    private SaveFileItem selectedSaveFile;

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
        // Hide Prompt Name Canvas
        if (promptNameCanvas != null) promptNameCanvas.Hide();

        // Clear dictionnary
        fileDico.Clear();

        // Clear AutoScroll
        autoScroll.Clear();

        // Instantiate "New Save" element in AutoScroll
        if (newsavePrefab != null)
        {
            GameObject _instance = autoScroll.AddPrefabReturnInstance(newsavePrefab);
            _instance.GetComponent<NewSaveFileButton>().OnSelection += UnselectItems;
            _instance.GetComponent<NewSaveFileButton>().onClick.AddListener(PromptNewFileName);
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
        // Check for existing file
        if (File.Exists(Path.Combine(Application.persistentDataPath, string.Concat(_saveName.Replace(' ', '_'), ".save"))))
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
}
