using System.Collections;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    // Public Canvas, one for each GameManager GameState
    public Canvas startMenu;
    public Canvas pauseMenu;
    public Canvas loadUI;
    public Canvas saveUI;
    public Canvas playUI;

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
        else if (loadUI == null)
        {
            Debug.LogError("[UIManager] No Load UI Canvas has been assigned");
        }
        else if (saveUI == null)
        {
            Debug.LogError("[UIManager] No Save UI Canvas has been assigned");
        }
        else if (playUI == null)
        {
            Debug.LogError("[UIManager] No Play UI Canvas has been assigned");
        }
        else
        {
            // Subscribe to all GameManager events to change and upadte UI
            GameManager.OnStartToLoad += ShowLoadUI;
            GameManager.OnLoadToPlay += ShowPlayUI;
            GameManager.OnPlayToStart += ShowStartMenu;
            GameManager.OnPlayToPause += ShowPauseMenu;
            GameManager.OnPauseToPlay += ShowPlayUI;
            GameManager.OnPauseToStart += ShowStartMenu;
            GameManager.OnPauseToLoad += ShowLoadUI;
            GameManager.OnPauseToSave += ShowSaveUI;
            GameManager.OnSaveToPause += ShowPauseMenu;

            GameManager.StartUpdate += StartMenu;
            GameManager.LoadUpdate += LoadUI;
            GameManager.PlayUpdate += PlayUI;
            GameManager.PauseUpdate += PauseMenu;
            GameManager.SaveUpdate += SaveUI;

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
        GameManager.OnStartToLoad -= ShowLoadUI;
        GameManager.OnLoadToPlay -= ShowPlayUI;
        GameManager.OnPlayToStart -= ShowStartMenu;
        GameManager.OnPlayToPause -= ShowPauseMenu;
        GameManager.OnPauseToPlay -= ShowPlayUI;
        GameManager.OnPauseToStart -= ShowStartMenu;
        GameManager.OnPauseToLoad -= ShowLoadUI;
        GameManager.OnPauseToSave -= ShowSaveUI;
        GameManager.OnSaveToPause -= ShowPauseMenu;
        GameManager.StartUpdate -= StartMenu;
        GameManager.LoadUpdate -= LoadUI;
        GameManager.PlayUpdate -= PlayUI;
        GameManager.PauseUpdate -= PauseMenu;
        GameManager.SaveUpdate -= SaveUI;
    }

    #region Show dedicate Canvas Methods
    /// <summary>
    /// InitializeUI method shows the first Canvas depending on the current GameState (the first)
    /// </summary>
    void InitializeUI()
    {
        switch (GameManager.CurrentGameState)
        {
            case GameManager.GameState.start:
                ShowStartMenu();
                break;
            case GameManager.GameState.load:
                ShowLoadUI();
                break;
            case GameManager.GameState.play:
                ShowPlayUI();
                break;
            case GameManager.GameState.pause:
                ShowPauseMenu();
                break;
            case GameManager.GameState.save:
                ShowSaveUI();
                break;
            default:
                ShowStartMenu();
                break;
        }
    }

    /// <summary>
    /// ShowStartMenu method activates the Start Menu and hides all the others
    /// </summary>
    void ShowStartMenu()
    {
        startMenu.gameObject.SetActive(true);
        pauseMenu.gameObject.SetActive(false);
        loadUI.gameObject.SetActive(false);
        saveUI.gameObject.SetActive(false);
        playUI.gameObject.SetActive(false);
    }

    /// <summary>
    /// ShowPauseMenu method activates the Pause Menu and hides all the others
    /// </summary>
    void ShowPauseMenu()
    {
        startMenu.gameObject.SetActive(false);
        pauseMenu.gameObject.SetActive(true);
        loadUI.gameObject.SetActive(false);
        saveUI.gameObject.SetActive(false);
        playUI.gameObject.SetActive(false);
    }

    /// <summary>
    /// ShowPlayUI method activates the Play UI and hides all the others
    /// </summary>
    void ShowPlayUI()
    {
        startMenu.gameObject.SetActive(false);
        pauseMenu.gameObject.SetActive(false);
        loadUI.gameObject.SetActive(false);
        saveUI.gameObject.SetActive(false);
        playUI.gameObject.SetActive(true);
    }

    /// <summary>
    /// ShowLoadUI method activates the Load UI and hides all the others
    /// </summary>
    void ShowLoadUI()
    {
        startMenu.gameObject.SetActive(false);
        pauseMenu.gameObject.SetActive(false);
        loadUI.gameObject.SetActive(true);
        saveUI.gameObject.SetActive(false);
        playUI.gameObject.SetActive(false);

        // For debug, until something is done in load and can change the GameState
        StartCoroutine(ChangeGameStateRequestDelayed(GameManager.GameState.play));
    }

    /// <summary>
    /// ShowSaveMenu method activates the Save UI and hides all the others
    /// </summary>
    void ShowSaveUI()
    {
        startMenu.gameObject.SetActive(false);
        pauseMenu.gameObject.SetActive(false);
        loadUI.gameObject.SetActive(false);
        saveUI.gameObject.SetActive(true);
        playUI.gameObject.SetActive(false);

        // For debug, until something is done in load and can change the GameState
        StartCoroutine(ChangeGameStateRequestDelayed(GameManager.GameState.pause));
    }

    /// <summary>
    /// Coroutine for debug to change the GameState after 5 seconds
    /// </summary>
    /// <param name="req">GameState requested to change to</param>
    /// <returns></returns>
    IEnumerator ChangeGameStateRequestDelayed(GameManager.GameState req)
    {
        yield return new WaitForSeconds(5f);
        GameManager.ChangeGameStateRequest(req);
    }
    #endregion

    /// <summary>
    /// Actions on each Canvas to switch from a GameState to another
    /// </summary>
    #region Canvas Actions
    void StartMenu()
    {
        if (Input.GetKeyDown(KeyCode.Space)) GameManager.ChangeGameStateRequest(GameManager.GameState.load);
        DebugModeShortcuts();
    }

    void PauseMenu()
    {
        if (Input.GetKeyDown(KeyCode.R)) GameManager.ChangeGameStateRequest(GameManager.GameState.load);
        else if (Input.GetKeyDown(KeyCode.Q)) GameManager.ChangeGameStateRequest(GameManager.GameState.start);
        else if (Input.GetKeyDown(KeyCode.S)) GameManager.ChangeGameStateRequest(GameManager.GameState.save);
        else if (Input.GetKeyDown(KeyCode.Escape)) GameManager.ChangeGameStateRequest(GameManager.GameState.play);
        DebugModeShortcuts();
    }

    void PlayUI()
    {
        if (Input.GetKeyDown(KeyCode.Q)) GameManager.ChangeGameStateRequest(GameManager.GameState.start);
        else if (Input.GetKeyDown(KeyCode.Escape)) GameManager.ChangeGameStateRequest(GameManager.GameState.pause);
        DebugModeShortcuts();
    }

    void LoadUI()
    {
        DebugModeShortcuts();
    }

    void SaveUI()
    {
        DebugModeShortcuts();
    }

    void DebugModeShortcuts()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) GameManager.ChangeGameStateRequest(GameManager.GameState.start);
        if (Input.GetKeyDown(KeyCode.Alpha2)) GameManager.ChangeGameStateRequest(GameManager.GameState.load);
        if (Input.GetKeyDown(KeyCode.Alpha3)) GameManager.ChangeGameStateRequest(GameManager.GameState.play);
        if (Input.GetKeyDown(KeyCode.Alpha4)) GameManager.ChangeGameStateRequest(GameManager.GameState.pause);
        if (Input.GetKeyDown(KeyCode.Alpha5)) GameManager.ChangeGameStateRequest(GameManager.GameState.save);
    }

    #endregion






}
