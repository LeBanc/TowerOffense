using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using System.Linq;

/// <summary>
/// ContinueButtonInit class setups the availabiliy of the Continue button of the Start menu
/// It has to be added to the same GameObject as the Button used for "Continue" game
/// </summary>
public class ContinueButtonInit : MonoBehaviour
{
    // Continue button
    private Button continueButton;
    // private Dictionary to store file data
    private Dictionary<DataSave.FileData, string> fileDico = new Dictionary<DataSave.FileData, string>();

    /// <summary>
    /// At Awake, fetches the Button and subscribe to event (for init when changing to Start GameState)
    /// </summary>
    private void Awake()
    {
        continueButton = GetComponent<Button>();
        if (continueButton == null) Debug.LogError("[ContinueButtonInit] Button was not found!");
        
        GameManager.OnPauseToStart += InitContinueButton;
        GameManager.OnPlayToStart += InitContinueButton;
    }

    /// <summary>
    /// OnDestroy, unsubscribe to events
    /// </summary>
    private void OnDestroy()
    {
        if (continueButton != null) continueButton.onClick.RemoveAllListeners();

        GameManager.OnPauseToStart -= InitContinueButton;
        GameManager.OnPlayToStart -= InitContinueButton;
    }

    /// <summary>
    /// InitContinueButton method searches for the last file save and, if one is found, set the click Action of the Continue button to a Load method
    /// </summary>
    private void InitContinueButton()
    {
#if UNITY_WEBGL
        return; // In WebGL, the continue button is hidden and shall not be init
#endif
        // Reset the action of continue button
        continueButton.onClick.RemoveAllListeners();

        string _saveFile = LoadSaveMenu.GetLastSavedFile();
        if (_saveFile.Equals(""))
        {
            // Reset the nav from the button below
            Navigation nav = continueButton.FindSelectableOnDown().navigation;
            nav.selectOnUp = null;
            continueButton.FindSelectableOnDown().navigation = nav;

            // hide the button
            gameObject.SetActive(false);
        }
        else
        {
            // Show the continue button
            gameObject.SetActive(true);

            // Set the nav from the button below
            Navigation nav = continueButton.FindSelectableOnDown().navigation;
            nav.selectOnUp = continueButton;
            continueButton.FindSelectableOnDown().navigation = nav;

            // Set the action of continue button
            continueButton.onClick.AddListener(delegate { GameManager.LoadGame(_saveFile); });
        }
    }
}
