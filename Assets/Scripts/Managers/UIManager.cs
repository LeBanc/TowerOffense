using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// UIManager is the manager of the UI
/// </summary>
public class UIManager : Singleton<UIManager>
{
    // Public Canvas, one for each GameManager GameState
    public UICanvas startMenu;
    public UICanvas pauseMenu;
    public UICanvas loadingUI;
    public UICanvas savingUI;
    public UICanvas playUI;
    public UICanvas loadMenu;
    public UICanvas saveMenu;
    public ConfirmMessageCanvas confirmMessage;
    public ErrorMessageCanvas errorMessage;

    private Selectable startMenuLastSelected;
    private Selectable pauseMenuLastSelected;
    private Selectable playUILastSelected;
    private Selectable loadMenuLastSelected;
    private Selectable saveMenuLastSelected;

    private LoadSaveMenu saveMenuComponent;

    public delegate void UIManagerEventHandler();
    public static event UIManagerEventHandler OnHideActiveCanvas;

    public static Selectable LastSelected
    {
        get { return Instance.playUILastSelected; }
        set { Instance.playUILastSelected = value; }
    }


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

            saveMenuComponent = saveMenu.GetComponent<LoadSaveMenu>();
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
        startMenu.Show();

        // Select default selectable
        if (startMenu.TryGetComponent<DefaultSelectable>(out DefaultSelectable _default))
        {
            if(_default.defaultSelectable != null) _default.defaultSelectable.Select();
        }
    }
    /// <summary>
    /// HideStartMenu method hides the Start Menu
    /// </summary>
    void HideStartMenu()
    {
        startMenu.Hide();
    }

    /// <summary>
    /// ShowPauseMenu method activates the Pause Menu
    /// </summary>
    void ShowPauseMenu()
    {
        pauseMenu.Show();

        // Select default selectable
        if (pauseMenu.TryGetComponent<DefaultSelectable>(out DefaultSelectable _default))
        {
            if (_default.defaultSelectable != null) _default.defaultSelectable.Select();
        }
    }
    /// <summary>
    /// HidePauseMenu method hides the Pause Menu
    /// </summary>
    void HidePauseMenu()
    {
        pauseMenu.Hide();
    }

    /// <summary>
    /// ShowPlayUI method activates the Play UI
    /// </summary>
    void ShowPlayUI()
    {
        playUI.Show();

        // Selection of default selectable is done by PlayUI dedicated canvas components
    }
    /// <summary>
    /// HidePlayUI method hides the Play UI
    /// </summary>
    void HidePlayUI()
    {
        playUI.Hide();
    }

    /// <summary>
    /// ShowLoadUI method activates the Loading UI
    /// </summary>
    void ShowLoadingUI()
    {
        loadingUI.Show();
    }
    /// <summary>
    /// HideLoadingUI method hides the Loading UI
    /// </summary>
    void HideLoadingUI()
    {
        loadingUI.Hide();
    }

    /// <summary>
    /// ShowSavingUI method activates the Saving UI
    /// </summary>
    void ShowSavingUI()
    {
        savingUI.Show();
    }
    /// <summary>
    /// HideSavingUI method hides the Saving UI
    /// </summary>
    void HideSavingUI()
    {
        savingUI.Hide();
    }

    /// <summary>
    /// ShowLoadMenu method activates the Load menu
    /// </summary>
    public void ShowLoadMenu()
    {
        // Save the last selected object on the previous canvas
        if(EventSystem.current.currentSelectedGameObject != null)
        {
            if (startMenu.IsVisible) startMenuLastSelected = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
            if (pauseMenu.IsVisible) pauseMenuLastSelected = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
        }

        loadMenu.Show();

    }
    /// <summary>
    /// HideLoadMenu method hides the Load Menu
    /// </summary>
    public static void HideLoadMenu()
    {
        Instance.loadMenu.Hide();

        // Select the last selected object from the previous canvas
        if (Instance.pauseMenu.IsVisible && Instance.pauseMenuLastSelected != null) Instance.pauseMenuLastSelected.Select();
        if (Instance.startMenu.IsVisible && Instance.startMenuLastSelected != null) Instance.startMenuLastSelected.Select();
    }

    /// <summary>
    /// ShowSaveMenu method activates the Save Menu
    /// </summary>
    public void ShowSaveMenu()
    {
        // Save the last selected object on the previous canvas
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            if (pauseMenu.IsVisible) pauseMenuLastSelected = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
        }

        saveMenu.Show();
    }
    /// <summary>
    /// HideSaveMenu method hides the Save Menu
    /// </summary>
    public static void HideSaveMenu()
    {
        Instance.saveMenu.Hide();
        // Select the last selected object from the previous canvas
        if (Instance.pauseMenu.IsVisible && Instance.pauseMenuLastSelected != null) Instance.pauseMenuLastSelected.Select();
    }
    #endregion

    #region Message Canvas
    /// <summary>
    /// InitConfirmMessage method displays the ConfirmMessage window with chosen parameters
    /// </summary>
    /// <param name="_message">Message to display (string)</param>
    /// <param name="_callback">Callback when validating the message - OK button (Action)</param>
    /// <param name="_sprite">(optionnal) Image to display (Sprite)</param>
    public static void InitConfirmMessage(string _message, Action _callback, Sprite _sprite = null)
    {
        // Save the last selected object on the previous canvas
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            if (Instance.loadMenu.IsVisible)
            {
                Instance.loadMenuLastSelected = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
            }
            else if (Instance.saveMenu.IsVisible)
            {
                Instance.saveMenuLastSelected = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
            }
            else if (Instance.startMenu.IsVisible)
            {
                Instance.startMenuLastSelected = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
            }
            else if (Instance.pauseMenu.IsVisible)
            {
                Instance.pauseMenuLastSelected = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
            }
            else if (Instance.playUI.IsVisible) Instance.playUILastSelected = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
        }            

        // Init the confirm message window
        Instance.confirmMessage.Show(_message, _callback, _sprite);
    }

    /// <summary>
    /// HideConfirmMessage method hides the confirm message window
    /// </summary>
    public static void HideConfirmMessage()
    {
        // Hide the window/canvas
        Instance.confirmMessage.Hide();

        // Select the last selected object from the previous canvas
        if (Instance.saveMenu.IsVisible && Instance.saveMenuLastSelected != null)
        {
            Instance.saveMenuLastSelected.Select();
        }
        else if(Instance.loadMenu.IsVisible && Instance.loadMenuLastSelected != null)
        {
            Instance.loadMenuLastSelected.Select();
        }
        else if(Instance.pauseMenu.IsVisible && Instance.pauseMenuLastSelected != null)
        {
            Instance.pauseMenuLastSelected.Select();
        }
        else if(Instance.startMenu.IsVisible && Instance.startMenuLastSelected != null)
        {
            Instance.startMenuLastSelected.Select();
        }
        else if(Instance.playUI.IsVisible && Instance.playUILastSelected != null)
        {
            Instance.playUILastSelected.Select();
        }
    }

    /// <summary>
    /// InitErrorMessage method init and shows an error message window
    /// </summary>
    /// <param name="_message">Message to display (string)</param>
    /// <param name="_sprite">Image to display (Sprite)</param>
    public static void InitErrorMessage(string _message, Sprite _sprite = null)
    {
        // Save the last selected object on the previous canvas
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            if (Instance.loadMenu.IsVisible)
            {
                Instance.loadMenuLastSelected = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
            }
            else if (Instance.saveMenu.IsVisible)
            {
                Instance.saveMenuLastSelected = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
            }
            else if (Instance.startMenu.IsVisible)
            {
                Instance.startMenuLastSelected = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
            }
            else if (Instance.pauseMenu.IsVisible)
            {
                Instance.pauseMenuLastSelected = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
            }
            else if (Instance.playUI.IsVisible) Instance.playUILastSelected = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
        }            

        // Display the error message
        Instance.errorMessage.Show(_message, _sprite);
    }

    /// <summary>
    /// HideErrorMessage method hides the error message window
    /// </summary>
    public static void HideErrorMessage()
    {
        // Hide the canvas
        Instance.errorMessage.Hide();

        // Select the last selected object from the previous canvas
        if (Instance.saveMenu.IsVisible && Instance.saveMenuLastSelected != null)
        {
            Instance.saveMenuLastSelected.Select();
        }
        else if (Instance.loadMenu.IsVisible && Instance.loadMenuLastSelected != null)
        {
            Instance.loadMenuLastSelected.Select();
        }
        else if (Instance.pauseMenu.IsVisible && Instance.pauseMenuLastSelected != null)
        {
            Instance.pauseMenuLastSelected.Select();
        }
        else if (Instance.startMenu.IsVisible && Instance.startMenuLastSelected != null)
        {
            Instance.startMenuLastSelected.Select();
        }
        else if (Instance.playUI.IsVisible && Instance.playUILastSelected != null)
        {
            Instance.playUILastSelected.Select();
        }
    }
    #endregion

    #region Pause Menu Quit actions
    /// <summary>
    /// ResumeGame method is used to go back to play mode state from a button on Pause menu
    /// </summary>
    public void ResumeGame()
    {
        GameManager.ChangeGameStateRequest(GameManager.GameState.play);
        if (playUILastSelected != null) playUILastSelected.Select();
    }

    /// <summary>
    /// QuitToStartMenu method is used to go back to start menu from a button on Pause menu
    /// </summary>
    public void QuitToStartMenu()
    {
        InitConfirmMessage("All data not saved will be loss, are you sure you want to go back to the main menu?", delegate { GameManager.ChangeGameStateRequest(GameManager.GameState.start); });
    }

    /// <summary>
    /// QuitGame method is used to quit the game from a button on Pause menu
    /// </summary>
    public void QuitGame()
    {
        InitConfirmMessage("All data not saved will be loss, are you sure you want to quit to desktop?", delegate { GameManager.QuitGameStatic(); });
    }

    #endregion

    #region Canvas Actions
    /// <summary>
    /// StartMenu method is call at Update when in Start gamestate
    /// </summary>
    void StartMenu()
    {
        // On Start menu, Esc. key is used to hide any window (confirm, error, load, save) or to quit the game
        if (Input.GetButtonDown("Cancel"))
        {
            if (confirmMessage.IsVisible)
            {
                HideConfirmMessage();
            }
            else if (errorMessage.IsVisible)
            {
                HideErrorMessage();
            }
            else if (saveMenu.IsVisible)
            {
                HideSaveMenu();
            }
            else if (loadMenu.IsVisible)
            {
                HideLoadMenu();
            }
            else if (Input.GetButtonDown("Pause"))
            {
                InitConfirmMessage("Are you sure you want to quit to desktop?", delegate { GameManager.QuitGameStatic(); });
            }
        }
        else if(Input.GetButtonDown("Pause"))
        {
            InitConfirmMessage("Are you sure you want to quit to desktop?", delegate { GameManager.QuitGameStatic(); });
        }
    }

    /// <summary>
    /// PauseMenu method is call at Update when in Pause gamestate
    /// </summary>
    void PauseMenu()
    {
        // On Pause menu, Esc. key is used to hide any window (confirm, error, load, save) or to return to play mode
        if (Input.GetButtonDown("Cancel"))
        {
            if(confirmMessage.IsVisible)
            {
                HideConfirmMessage();
            }
            else if (errorMessage.IsVisible)
            {
                HideErrorMessage();
            }
            else if(saveMenu.IsVisible)
            {
                if(saveMenuComponent.promptNameCanvas.IsVisible)
                {
                    saveMenuComponent.promptNameCanvas.Hide();
                }
                else
                {
                    HideSaveMenu();
                }                
            }
            else if(loadMenu.IsVisible)
            {
                HideLoadMenu();
            }
            else
            {
                GameManager.ChangeGameStateRequest(GameManager.GameState.play);
                if (playUILastSelected != null)
                {
                    playUILastSelected.Select();
                    playUILastSelected = null;
                }
                else
                {
                    EventSystem.current.SetSelectedGameObject(null);
                }
            }            
        }
        else if(Input.GetButtonDown("Pause"))
        {
            if (confirmMessage.IsVisible) confirmMessage.Hide();
            if (errorMessage.IsVisible) errorMessage.Hide();
            if (saveMenu.IsVisible)
            {
                if (saveMenuComponent.promptNameCanvas.IsVisible)
                {
                    saveMenuComponent.SetPromptNameCanvaseEvent(false);
                    saveMenuComponent.promptNameCanvas.Hide();
                    saveMenuComponent.SetPromptNameCanvaseEvent(true);
                }
                saveMenu.Hide();
            }
            if (loadMenu.IsVisible) loadMenu.Hide();
            
            GameManager.ChangeGameStateRequest(GameManager.GameState.play);
            if (playUILastSelected != null)
            {
                Debug.Log(playUILastSelected.name);
                playUILastSelected.Select();
                playUILastSelected = null;
            }
            else
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
        }
    }

    /// <summary>
    /// PlayUI method is call at Update when in Play gamestate
    /// </summary>
    void PlayUI()
    {
        // On Play UI, Esc. key is used to hide any window (confirm, error) or to go to pause Menu (via pause gamestate)
        if (Input.GetButtonDown("Cancel"))
        {
            if (OnHideActiveCanvas != null)
            {
                OnHideActiveCanvas?.Invoke();
            }
            else if (confirmMessage.IsVisible)
            {
                HideConfirmMessage();
            }
            else if (errorMessage.IsVisible)
            {
                HideErrorMessage();
            }
            else if(Input.GetButtonDown("Pause"))
            {
                if (EventSystem.current.currentSelectedGameObject != null) playUILastSelected = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
                GameManager.ChangeGameStateRequest(GameManager.GameState.pause);
            }
        }
        else if(Input.GetButtonDown("Pause"))
        {
            if (EventSystem.current.currentSelectedGameObject != null) playUILastSelected = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
            GameManager.ChangeGameStateRequest(GameManager.GameState.pause);
        }
    }

    /// <summary>
    /// LoadingUI method is call at Update when in Load gamestate
    /// </summary>
    void LoadingUI()
    {
        
    }

    /// <summary>
    /// SavingUI method is call at Update when in Save gamestate
    /// </summary>
    void SavingUI()
    {
        
    }
    #endregion
}
