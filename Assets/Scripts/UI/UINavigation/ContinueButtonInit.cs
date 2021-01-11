using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using System.Linq;

public class ContinueButtonInit : MonoBehaviour
{
    private Button continueButton;
    private Dictionary<DataSave.FileData, string> fileDico = new Dictionary<DataSave.FileData, string>();

    private void Awake()
    {
        continueButton = GetComponent<Button>();
        if (continueButton == null) Debug.LogError("[ContinueButtonInit] Button was not found!");
        
        GameManager.OnPauseToStart += InitContinueButton;
        GameManager.OnPlayToStart += InitContinueButton;
    }

    private void OnDestroy()
    {
        if (continueButton != null) continueButton.onClick.RemoveAllListeners();

        GameManager.OnPauseToStart -= InitContinueButton;
        GameManager.OnPlayToStart -= InitContinueButton;
    }

    private void InitContinueButton()
    {
        // Clear dictionnary
        fileDico.Clear();

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

        // If there is a save, take the first one (the last saved) and show the continue button
        if(_fileDataList.Count > 0)
        {
            string _fileName = "";
            if(fileDico.TryGetValue(_fileDataList[0], out _fileName))
            {
                // Show the continue button
                gameObject.SetActive(true);

                // Set the nav from the button below
                Navigation nav = continueButton.FindSelectableOnDown().navigation;
                nav.selectOnUp = continueButton;
                continueButton.FindSelectableOnDown().navigation = nav;

                // Set the action of continue button
                continueButton.onClick.AddListener(delegate { GameManager.LoadGame(_fileName); });
            }
        }
        else // hide the continue button
        {
            // Reset the nav from the button below
            Navigation nav = continueButton.FindSelectableOnDown().navigation;
            nav.selectOnUp = null;
            continueButton.FindSelectableOnDown().navigation = nav;

            // Reset the action of continue button
            continueButton.onClick.RemoveAllListeners();

            // hide the button
            gameObject.SetActive(false);
        }
    }
}
