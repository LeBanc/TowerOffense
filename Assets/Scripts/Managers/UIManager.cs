using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    public Canvas startMenu;
    public Canvas pauseMenu;
    public Canvas loadUI;
    public Canvas saveUI;
    public Canvas playUI;

    // Start is called before the first frame update
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
            InitializeUI();
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
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


    void ShowStartMenu()
    {
        startMenu.gameObject.SetActive(true);
        pauseMenu.gameObject.SetActive(false);
        loadUI.gameObject.SetActive(false);
        saveUI.gameObject.SetActive(false);
        playUI.gameObject.SetActive(false);
    }

    void ShowPauseMenu()
    {
        startMenu.gameObject.SetActive(false);
        pauseMenu.gameObject.SetActive(true);
        loadUI.gameObject.SetActive(false);
        saveUI.gameObject.SetActive(false);
        playUI.gameObject.SetActive(false);
    }

    void ShowPlayUI()
    {
        startMenu.gameObject.SetActive(false);
        pauseMenu.gameObject.SetActive(false);
        loadUI.gameObject.SetActive(false);
        saveUI.gameObject.SetActive(false);
        playUI.gameObject.SetActive(true);
    }

    void ShowLoadUI()
    {
        startMenu.gameObject.SetActive(false);
        pauseMenu.gameObject.SetActive(false);
        loadUI.gameObject.SetActive(true);
        saveUI.gameObject.SetActive(false);
        playUI.gameObject.SetActive(false);

        // For debug
        StartCoroutine(ChangeGameStateRequestDelayed(GameManager.GameState.play));
    }

    void ShowSaveUI()
    {
        startMenu.gameObject.SetActive(false);
        pauseMenu.gameObject.SetActive(false);
        loadUI.gameObject.SetActive(false);
        saveUI.gameObject.SetActive(true);
        playUI.gameObject.SetActive(false);

        // For debug
        StartCoroutine(ChangeGameStateRequestDelayed(GameManager.GameState.pause));
    }

    IEnumerator ChangeGameStateRequestDelayed(GameManager.GameState req)
    {
        yield return new WaitForSeconds(5f);
        GameManager.ChangeGameStateRequest(req);
    }
    #endregion

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
