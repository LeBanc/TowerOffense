using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIManager : Singleton<UIManager>
{
    // Public Canvas, one for each GameManager GameState
    public Canvas startMenu;
    public Canvas pauseMenu;
    public Canvas loadingUI;
    public Canvas savingUI;
    public Canvas playUI;
    public Canvas loadMenu;
    public Canvas saveMenu;
    public ConfirmMessageCanvas confirmMessage;
    public ErrorMessageCanvas errorMessage;

    private Selectable startMenuLastSelected;
    private Selectable pauseMenuLastSelected;
    private Selectable playUILastSelected;
    private Selectable loadMenuLastSelected;
    private Selectable saveMenuLastSelected;

    /// <summary>
    /// At Start, checks for missing UI Canvas and subscribes to GameManager events
    /// </summary>
    void Start()
    {
        // Default checks
        if (startMenu == null)
        {
            Debug.LogError("[UIManager] No Start Menu Canvas has been assigned");
        }
        else if (pauseMenu == null)
        {
            Debug.LogError("[UIManager] No Pause Menu Canvas has been assigned");
        }
         else if (playUI == null)
        {
            Debug.LogError("[UIManager] No Play UI Canvas has been assigned");
        }
        else if (savingUI == null)
        {
            Debug.LogError("[UIManager] No Saving UI Canvas has been assigned");
        }
        else if (loadingUI == null)
        {
            Debug.LogError("[UIManager] No Loading UI Canvas has been assigned");
        }
        else
        {
            // Subscribe to all GameManager events to change and upadte UI
            GameManager.OnStartToLoad += ShowLoadingUI;
            GameManager.OnStartToLoad += HideStartMenu;
            GameManager.OnStartToLoad += HideLoadMenu;

            GameManager.OnLoadToPlay += ShowPlayUI;
            GameManager.OnLoadToPlay += HideLoadingUI;

            GameManager.OnPlayToPause += ShowPauseMenu;

            GameManager.OnPlayToSave += ShowSavingUI;

            GameManager.OnPlayToStart += ShowStartMenu;
            GameManager.OnPlayToStart += HidePlayUI;

            GameManager.OnPauseToPlay += HidePauseMenu;

            GameManager.OnPauseToStart += ShowStartMenu;
            GameManager.OnPauseToStart += HidePauseMenu;

            GameManager.OnPauseToLoad += ShowLoadingUI;
            GameManager.OnPauseToLoad += HidePauseMenu;
            GameManager.OnPauseToLoad += HideLoadMenu;

            GameManager.OnPauseToSave += ShowSavingUI;
            GameManager.OnPauseToSave += HideSaveMenu;

            GameManager.OnSaveToPause += HideSavingUI;
            GameManager.OnSaveToPlay += HideSavingUI;

            GameManager.StartUpdate += StartMenu;
            GameManager.LoadUpdate += LoadingUI;
            GameManager.PlayUpdate += PlayUI;
            GameManager.PauseUpdate += PauseMenu;
            GameManager.SaveUpdate += SavingUI;

            // Initialize UI from current GameState
            InitializeUI();
        }
    }

    /// <summary>
    /// OnDestroy methods delete the UIManager and ununscribes from GameManager events
    /// </summary>
    protected override void OnDestroy()
    {
        base.OnDestroy();

        // Unsubscribe from GameManager events
        GameManager.OnStartToLoad -= ShowLoadingUI;
        GameManager.OnStartToLoad -= HideStartMenu;
        GameManager.OnLoadToPlay -= ShowPlayUI;
        GameManager.OnLoadToPlay -= HideLoadingUI;
        GameManager.OnPlayToPause -= ShowPauseMenu;
        GameManager.OnPlayToSave -= ShowSavingUI;
        GameManager.OnPlayToStart -= ShowStartMenu;
        GameManager.OnPlayToStart -= HidePlayUI;
        GameManager.OnPauseToPlay -= HidePauseMenu;
        GameManager.OnPauseToStart -= ShowStartMenu;
        GameManager.OnPauseToStart -= HidePauseMenu;
        GameManager.OnPauseToLoad -= ShowLoadingUI;
        GameManager.OnPauseToLoad -= HidePauseMenu;
        GameManager.OnPauseToSave -= ShowSavingUI;
        GameManager.OnSaveToPause -= HideSavingUI;
        GameManager.OnSaveToPlay -= HideSavingUI;

        GameManager.StartUpdate -= StartMenu;
        GameManager.LoadUpdate -= LoadingUI;
        GameManager.PlayUpdate -= PlayUI;
        GameManager.PauseUpdate -= PauseMenu;
        GameManager.SaveUpdate -= SavingUI;
        
        GameManager.OnStartToLoad -= HideLoadMenu;
        GameManager.OnPauseToLoad -= HideLoadMenu;
        GameManager.OnPauseToSave -= HideSaveMenu;
    }

    #region Show dedicate Canvas Methods
    /// <summary>
    /// InitializeUI method hides all the canvases
    /// </summary>
    void InitializeUI()
    {
        startMenu.gameObject.SetActive(true);
        pauseMenu.gameObject.SetActive(true);
        loadingUI.gameObject.SetActive(true);
        savingUI.gameObject.SetActive(true);
        playUI.gameObject.SetActive(true);
        loadMenu.gameObject.SetActive(true);
        saveMenu.gameObject.SetActive(true);

    HideStartMenu();
        HideLoadingUI();
        HidePlayUI();
        HidePauseMenu();
        HideSavingUI();
        HideLoadMenu();
        HideSaveMenu();
    }

    /// <summary>
    /// ShowStartMenu method activates the Start Menu
    /// </summary>
    void ShowStartMenu()
    {
        startMenu.enabled = true;

        // Select default selectable
        if (startMenu.TryGetComponent<DefaultSelectable>(out DefaultSelectable _default))
        {
            _default.defaultSelectable.Select();
        }
    }
    /// <summary>
    /// HideStartMenu method hides the Start Menu
    /// </summary>
    void HideStartMenu()
    {
        startMenu.enabled = false;
    }

    /// <summary>
    /// ShowPauseMenu method activates the Pause Menu
    /// </summary>
    void ShowPauseMenu()
    {
        pauseMenu.enabled=true;

        // Select default selectable
        if (pauseMenu.TryGetComponent<DefaultSelectable>(out DefaultSelectable _default))
        {
            _default.defaultSelectable.Select();
        }
    }
    /// <summary>
    /// HidePauseMenu method hides the Pause Menu
    /// </summary>
    void HidePauseMenu()
    {
        pauseMenu.enabled=false;
    }

    /// <summary>
    /// ShowPlayUI method activates the Play UI
    /// </summary>
    void ShowPlayUI()
    {
        playUI.enabled=true;

        // Selection of default selectable is done by PlayUI dedicated canvas components
    }
    /// <summary>
    /// HidePlayUI method hides the Play UI
    /// </summary>
    void HidePlayUI()
    {
        playUI.enabled=false;
    }

    /// <summary>
    /// ShowLoadUI method activates the Loading UI
    /// </summary>
    void ShowLoadingUI()
    {
        loadingUI.enabled=true;
    }
    /// <summary>
    /// HideLoadingUI method hides the Loading UI
    /// </summary>
    void HideLoadingUI()
    {
        loadingUI.enabled=false;
    }

    /// <summary>
    /// ShowSavingUI method activates the Saving UI
    /// </summary>
    void ShowSavingUI()
    {
        savingUI.enabled=true;
        savingUI.GetComponent<SaveAnimation>().StartAnimation();
    }
    /// <summary>
    /// HideSavingUI method hides the Saving UI
    /// </summary>
    void HideSavingUI()
    {
        savingUI.GetComponent<SaveAnimation>().StopAnimation();
        savingUI.enabled=false;
    }

    /// <summary>
    /// ShowLoadMenu method activates the Load menu
    /// </summary>
    public void ShowLoadMenu()
    {
        if(EventSystem.current.currentSelectedGameObject != null)
        {
            if (startMenu.enabled) startMenuLastSelected = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
            if (pauseMenu.enabled) pauseMenuLastSelected = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
        }       

        loadMenu.enabled = true;
        loadMenu.GetComponentInChildren<LoadSaveMenu>().SetupLoadMenu();

    }
    /// <summary>
    /// HideLoadMenu method hides the Load Menu
    /// </summary>
    public void HideLoadMenu()
    {
        loadMenu.enabled = false;
        if (pauseMenu.enabled && pauseMenuLastSelected != null) pauseMenuLastSelected.Select();
        if (startMenu.enabled && startMenuLastSelected != null) startMenuLastSelected.Select();
    }

    /// <summary>
    /// ShowSaveMenu method activates the Save Menu
    /// </summary>
    public void ShowSaveMenu()
    {
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            if (pauseMenu.enabled) pauseMenuLastSelected = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
        }            

        saveMenu.enabled = true;
        saveMenu.GetComponentInChildren<LoadSaveMenu>().SetupSaveMenu();
    }
    /// <summary>
    /// HideSaveMenu method hides the Save Menu
    /// </summary>
    public void HideSaveMenu()
    {
        saveMenu.enabled = false;
        if (pauseMenu.enabled && pauseMenuLastSelected != null) pauseMenuLastSelected.Select();
    }
    #endregion

    #region Message Canvas
    public static void InitConfirmMessage(string _message, Action _callback, Sprite _sprite = null)
    {
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            if (Instance.loadMenu.enabled)
            {
                Instance.loadMenuLastSelected = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
            }
            else if (Instance.saveMenu.enabled)
            {
                Instance.saveMenuLastSelected = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
            }
            else if (Instance.startMenu.enabled)
            {
                Instance.startMenuLastSelected = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
            }
            else if (Instance.pauseMenu.enabled)
            {
                Instance.pauseMenuLastSelected = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
            }
            else if (Instance.playUI.enabled) Instance.playUILastSelected = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
        }            

        Instance.confirmMessage.Show(_message, _callback, _sprite);
    }

    public static void HideConfirmMessage()
    {
        Instance.confirmMessage.Hide();

        if(Instance.saveMenu.enabled && Instance.saveMenuLastSelected != null)
        {
            Instance.saveMenuLastSelected.Select();
        }
        else if(Instance.loadMenu.enabled && Instance.loadMenuLastSelected != null)
        {
            Instance.loadMenuLastSelected.Select();
        }
        else if(Instance.pauseMenu.enabled && Instance.pauseMenuLastSelected != null)
        {
            Instance.pauseMenuLastSelected.Select();
        }
        else if(Instance.startMenu.enabled && Instance.startMenuLastSelected != null)
        {
            Instance.startMenuLastSelected.Select();
        }
        else if(Instance.playUI.enabled && Instance.playUILastSelected != null)
        {
            Instance.playUILastSelected.Select();
        }
    }

    public static void InitErrorMessage(string _message, Sprite _sprite = null)
    {
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            if (Instance.loadMenu.enabled)
            {
                Instance.loadMenuLastSelected = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
            }
            else if (Instance.saveMenu.enabled)
            {
                Instance.saveMenuLastSelected = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
            }
            else if (Instance.startMenu.enabled)
            {
                Instance.startMenuLastSelected = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
            }
            else if (Instance.pauseMenu.enabled)
            {
                Instance.pauseMenuLastSelected = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
            }
            else if (Instance.playUI.enabled) Instance.playUILastSelected = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
        }            

        Instance.errorMessage.Show(_message, _sprite);
    }

    public static void HideErrorMessage()
    {
        Instance.errorMessage.Hide();

        if (Instance.saveMenu.enabled && Instance.saveMenuLastSelected != null)
        {
            Instance.saveMenuLastSelected.Select();
        }
        else if (Instance.loadMenu.enabled && Instance.loadMenuLastSelected != null)
        {
            Instance.loadMenuLastSelected.Select();
        }
        else if (Instance.pauseMenu.enabled && Instance.pauseMenuLastSelected != null)
        {
            Instance.pauseMenuLastSelected.Select();
        }
        else if (Instance.startMenu.enabled && Instance.startMenuLastSelected != null)
        {
            Instance.startMenuLastSelected.Select();
        }
        else if (Instance.playUI.enabled && Instance.playUILastSelected != null)
        {
            Instance.playUILastSelected.Select();
        }
    }
    #endregion

    #region Pause Menu Quit actions
    public void ResumeGame()
    {
        GameManager.ChangeGameStateRequest(GameManager.GameState.play);
        if (playUILastSelected != null) playUILastSelected.Select();
    }
    
    public void QuitToStartMenu()
    {
        InitConfirmMessage("All data not saved will be loss, are you sure you want to go back to the main menu?", delegate { GameManager.ChangeGameStateRequest(GameManager.GameState.start); });
    }

    public void QuitGame()
    {
        InitConfirmMessage("All data not saved will be loss, are you sure you want to quit to desktop?", delegate { GameManager.QuitGameStatic(); });
    }

    #endregion

    /// <summary>
    /// Actions on each Canvas to switch from a GameState to another
    /// </summary>
    #region Canvas Actions
    void StartMenu()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (confirmMessage.IsShown)
            {
                HideConfirmMessage();
            }
            else if (errorMessage.IsShown)
            {
                HideErrorMessage();
            }
            else if (saveMenu.enabled == true)
            {
                HideSaveMenu();
            }
            else if (loadMenu.enabled == true)
            {
                HideLoadMenu();
            }
            else
            {
                InitConfirmMessage("Are you sure you want to quit to desktop?", delegate { GameManager.QuitGameStatic(); });
            }            
        }
    }

    void PauseMenu()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(confirmMessage.IsShown)
            {
                HideConfirmMessage();
            }
            else if (errorMessage.IsShown)
            {
                HideErrorMessage();
            }
            else if(saveMenu.enabled == true)
            {
                HideSaveMenu();
            }
            else if(loadMenu.enabled == true)
            {
                HideLoadMenu();
            }
            else
            {
                GameManager.ChangeGameStateRequest(GameManager.GameState.play);
                if (playUILastSelected != null) playUILastSelected.Select();
            }            
        }
    }

    void PlayUI()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(confirmMessage.IsShown)
            {
                HideConfirmMessage();
            }
            else if (errorMessage.IsShown)
            {
                HideErrorMessage();
            }
            else
            {
                if (EventSystem.current.currentSelectedGameObject != null) playUILastSelected = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
                GameManager.ChangeGameStateRequest(GameManager.GameState.pause);
            }
        }            
    }

    void LoadingUI()
    {
        
    }

    void SavingUI()
    {
        
    }
    #endregion
}
