using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class LoadSaveMenu : MonoBehaviour
{
    public AutoScroll autoScroll;
    public GameObject savefileitemPrefab;

    public GameObject newsavePrefab;
    public SavePromptNameCanvas promptNameCanvas;

    private Dictionary<DataSave.FileData, string> fileDico = new Dictionary<DataSave.FileData, string>();
    private SaveFileItem selectedSaveFile;

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

    private void SelectItem(SaveFileItem _sfi)
    {
        // If same selected item, return
        if (selectedSaveFile == _sfi) return;
        // Else, if an item is selected, unselect it
        if (selectedSaveFile != null) selectedSaveFile.Unselect();
        // Set the new item as the selected one
        selectedSaveFile = _sfi;
    }

    private void UnselectItems()
    {
        if (selectedSaveFile != null) selectedSaveFile.Unselect();
        selectedSaveFile = null;
    }

    public void LoadFile()
    {
        if (selectedSaveFile != null)
        {
            GameManager.LoadGame(selectedSaveFile.FileName);            
        }
    }

    public void PromptNewFileName()
    {
        if (promptNameCanvas != null)
        {
            promptNameCanvas.OnValidate += CreateNewSaveFile;
            promptNameCanvas.Show();
        }
    }

    public void CreateNewSaveFile(string _saveName)
    {
        // Check for existing file
        if (File.Exists(Path.Combine(Application.persistentDataPath, string.Concat(_saveName.Replace(' ', '_'), ".save"))))
        {
            // If exist => overwrite message with Save on confirm
            UIManager.InitConfirmMessage("The file already exists, are you sure you want to overwrite it?", delegate {
                StartCoroutine(SaveGame(_saveName));
                //GameManager.ChangeGameStateRequest(GameManager.GameState.save);
                //new DataSave().SaveGame(_saveName);                
            });
        }
        else
        {
            // Else Save
            StartCoroutine(SaveGame(_saveName));
            //new DataSave().SaveGame(_saveName);
        }
    }

    IEnumerator SaveGame(string _saveName)
    {
        float startTime = Time.time;
        GameManager.ChangeGameStateRequest(GameManager.GameState.save);
        new DataSave().SaveGame(_saveName);
        while(Time.time < startTime +1f)
        {
            yield return null;
        }
        GameManager.ChangeGameStateRequest(GameManager.GameState.pause);
    }

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
