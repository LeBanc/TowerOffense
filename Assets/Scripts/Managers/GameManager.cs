﻿using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEngine.EventSystems;

/// <summary>
/// GameManager class is the base manager of the game
/// </summary>
public class GameManager : Singleton<GameManager>
{
    /// <summary>
    /// There are 6 GameStates defined. It is possible know the current GameState and to request a change of GameState
    /// </summary>
    #region GameState
    public enum GameState
    {
        init,
        start,
        load,
        play,
        pause,
        save
    }

    [SerializeField] // For development, when not starting the game from Main Menu
    private bool debugBeginInPlayMode = false;

    private static GameState currentGameState;
    //private static GameState previousGameState;
    private static GameState nextGameState;

    public static GameState CurrentGameState
    {
        get { return currentGameState; }
    }

    public static void ChangeGameStateRequest(GameState next)
    {
        nextGameState = next;
    }
    #endregion

    #region Events

    public delegate void GameStateEventHandler();

    // Events for GameState change
    public static event GameStateEventHandler OnStartToLoad;

    public static event GameStateEventHandler OnLoadToPlay;

    public static event GameStateEventHandler OnPlayToPause;
    public static event GameStateEventHandler OnPlayToSave;
    public static event GameStateEventHandler OnPlayToStart;

    public static event GameStateEventHandler OnPauseToSave;
    public static event GameStateEventHandler OnPauseToPlay;
    public static event GameStateEventHandler OnPauseToLoad;
    public static event GameStateEventHandler OnPauseToStart;

    public static event GameStateEventHandler OnSaveToPause;
    public static event GameStateEventHandler OnSaveToPlay;

    // Events called in GameState to Update components
    public static event GameStateEventHandler StartUpdate;
    public static event GameStateEventHandler LoadUpdate;
    public static event GameStateEventHandler PlayUpdate;
    public static event GameStateEventHandler PauseUpdate;
    public static event GameStateEventHandler SaveUpdate;

    #endregion

    /// <summary>
    /// List of managers to instantiate (public) and method to instantiate/clear them or add/remove one
    /// </summary>
    #region Managers

    public GameObject[] managers;
    private List<GameObject> _instanciatedManagers;

    /// <summary>
    /// InstantiateManagers method instantiates the managers set as parameters
    /// </summary>
    private void InstantiateManagers()
    {
        if (managers.Length > 0)
        {
            GameObject _prefabInstanciated;
            for (int i = 0; i < managers.Length; i++)
            {
                _prefabInstanciated = Instantiate(managers[i]);
                _instanciatedManagers.Add(_prefabInstanciated);
            }
        }
    }

    /// <summary>
    /// ClearInstantiatedManagers method clears the instantiated managers
    /// </summary>
    private void ClearInstantiatedManagers()
    {
        foreach (var go in _instanciatedManagers)
        {
            Destroy(go);
        }
        _instanciatedManagers.Clear();
    }

    /// <summary>
    /// AddManager method adds a manager to the GameManager list
    /// </summary>
    /// <param name="manager"></param>
    private void AddManager(GameObject manager)
    {
        _instanciatedManagers.Add(manager);
    }

    /// <summary>
    /// RemoveManager method removes a dedicated manager from the GameManager list
    /// </summary>
    /// <param name="manager"></param>
    private void RemoveManager(GameObject manager)
    {
        if (_instanciatedManagers.Contains(manager))
        {
            _instanciatedManagers.Remove(manager);
            Destroy(manager);
        }
        else
        {
            Debug.LogError("[GameManager] Trying to delete an unknown manager: " + manager.name);
        }
    }
    #endregion


    /// <summary>
    /// Methods to load/unload scene (AsyncOperation)
    /// </summary>
    #region Scenes

    private List<AsyncOperation> _loadOperations = new List<AsyncOperation>();
    private List<AsyncOperation> _unloadOperations = new List<AsyncOperation>();

    /// <summary>
    /// LoadLevel method adds the "load scene" operation to the loading async operation list
    /// </summary>
    /// <param name="sceneName">Scene to load (string)</param>
    public void LoadLevel(string sceneName)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        _loadOperations.Add(ao);
    }

    /// <summary>
    /// UnloadLevel method adds the "unload scene" operation to the unloading async operation list
    /// </summary>
    /// <param name="sceneName">Scene to unload (string)</param>
    public void UnloadLevel(string sceneName)
    {
        AsyncOperation ao = SceneManager.UnloadSceneAsync(sceneName);
        _unloadOperations.Add(ao);
    }

    /// <summary>
    /// ClearLoadedLevel method check witch scene is loaded and unload it
    /// </summary>
    public void ClearLoadedLevel()
    {
        if (SceneManager.GetSceneByName("Test").IsValid()) UnloadLevel("Test");
        if (SceneManager.GetSceneByName("EmptyScene").IsValid()) UnloadLevel("EmptyScene");
    }

    /// <summary>
    /// StartNewGame method launch the StartCoroutine for a new game
    /// </summary>
    public void StartNewGame()
    {
        Instance.StartCoroutine(LoadingSequence(0));
    }

    /// <summary>
    /// LoadGame method launch the StartCoroutine for a saved game
    /// </summary>
    /// <param name="_fileName">Saved game's file name (string)</param>
    public static void LoadGame(string _fileName)
    {
        Instance.StartCoroutine(Instance.LoadingSequence(1,_fileName));
    }

    /// <summary>
    /// LoadingSequence coroutine load a saved game or a new game
    /// </summary>
    /// <param name="_loadingType">0 for a new game, 1 for a saved game (int)</param>
    /// <param name="_fileName">name a the file to load, only used when _loadingType == 1 (string, default is "")</param>
    /// <returns></returns>
    private IEnumerator LoadingSequence(int _loadingType, string _fileName = "")
    {
        float startTime = Time.time;
        // Change game state
        ChangeGameStateRequest(GameState.load);

        // Unload currently loaded level (if any)
        ClearLoadedLevel();
        if (_unloadOperations.Count > 0)
        {
            while (_unloadOperations[_unloadOperations.Count - 1].progress < 1f)
            {
                yield return null;
            }
        }

        yield return new WaitForSeconds(2f);

        // Load level (Test for new game, EmptyScene for loaded game)
        if (_loadingType == 0)
        {
            if (!SceneManager.GetSceneByName("Test").IsValid() && !SceneManager.GetSceneByName("EmptyScene").IsValid()) LoadLevel("Test");
        }
        else
        {
            if (!SceneManager.GetSceneByName("Test").IsValid() && !SceneManager.GetSceneByName("EmptyScene").IsValid()) Instance.LoadLevel("EmptyScene");
        }
        if (_loadOperations.Count > 0)
        {
            while (_loadOperations[_loadOperations.Count - 1].progress < 1f)
            {
                yield return null;
            }
        }
        
        //Setup PlayManager data
        PlayManager.LoadFromEmptyScene();
        if(_loadingType == 0)
        {
            PlayManager.LoadFromScene();
        }
        else if(_loadingType == 1)
        {
            DataSave.LoadSavedGame(_fileName);
        }
        // Always init after a load to unsure the game is in HQPhase, showing the new loaded data
        PlayManager.InitAfterLoad(_loadingType==0);

        // Small pause if the loading take less than 2 seconds, just to see the loading screen
        while (Time.time < startTime +2f)
        {
            yield return null;
        }
        ChangeGameStateRequest(GameState.play);
    }
    #endregion

    /// <summary>
    /// At Start, initialize the GameState values and instatiate the chosen Managers
    /// </summary>
    void Start()
    {
        //previousGameState = GameState.init;
        currentGameState = GameState.init;
        nextGameState = GameState.init;
        _instanciatedManagers = new List<GameObject>();
        InstantiateManagers();

        OnPauseToStart += ClearLoadedLevel;
        OnPlayToStart += ClearLoadedLevel;
    }

    /// <summary>
    /// Update is called once per frame, after changing the GameState if needed, it calls the event dedicated to the current GameState 
    /// It is (or should be) the only Update call of all Component in the project. The other elements should subscribe to the GameManager events
    /// </summary>
    void Update()
    {
        // Change state if needed
        if (currentGameState != nextGameState || currentGameState == GameState.init) ChangeGameState();

        // Basic state machine
        switch (GameManager.currentGameState)
        {
            case GameState.init:
                break;
            case GameState.start:
                StartUpdate?.Invoke();
                break;
            case GameState.load:
                LoadUpdate?.Invoke();
                break;
            case GameState.play:
                PlayUpdate?.Invoke();
                break;
            case GameState.pause:
                PauseUpdate?.Invoke();
                break;
            case GameState.save:
                SaveUpdate?.Invoke();
                break;
            default:
                Debug.LogError("[GameManager] Unknown GameState");
                break;
        }
    }

    /// <summary>
    /// ChangeGameState method is a state machine of GameState changes.
    /// It switches from currentGameState to nextGameState only if the transition is allowed
    /// </summary>
    void ChangeGameState()
    {
        switch (GameManager.currentGameState)
        {
            case GameState.init:
                if(debugBeginInPlayMode == true){ OnLoadToPlay?.Invoke(); currentGameState = GameState.play; nextGameState = GameState.play; }
                else { OnPlayToStart?.Invoke(); currentGameState = GameState.start; nextGameState = GameState.start; }
                break;
            case GameState.start:
                if (nextGameState == GameState.load) { OnStartToLoad?.Invoke(); currentGameState = nextGameState; }
                else { Debug.LogError("[GameManager] Cannot transition from Start to " + nextGameState); nextGameState = currentGameState; }
                break;
            case GameState.load:
                if (nextGameState == GameState.play) {OnLoadToPlay?.Invoke(); currentGameState = nextGameState; }
                else {Debug.LogError("[GameManager] Cannot transition from Load to " + nextGameState); nextGameState = currentGameState; }
                break;
            case GameState.play:
                if (nextGameState == GameState.pause) {OnPlayToPause?.Invoke(); currentGameState = nextGameState; }
                else if (nextGameState == GameState.save) { OnPlayToSave?.Invoke(); currentGameState = nextGameState; }
                else if (nextGameState == GameState.start) {OnPlayToStart?.Invoke(); currentGameState = nextGameState; }
                else {Debug.LogError("[GameManager] Cannot transition from Play to " + nextGameState); nextGameState = currentGameState; }
                break;
            case GameState.pause:
                if (nextGameState == GameState.start) {OnPauseToStart?.Invoke(); currentGameState = nextGameState; Time.timeScale = 1f; }
                else if (nextGameState == GameState.play) {OnPauseToPlay?.Invoke(); currentGameState = nextGameState; }
                else if (nextGameState == GameState.save) {OnPauseToSave?.Invoke(); currentGameState = nextGameState; Time.timeScale = 1f; }
                else if (nextGameState == GameState.load) { OnPauseToLoad?.Invoke(); currentGameState = nextGameState; Time.timeScale = 1f; }
                else {Debug.LogError("[GameManager] Cannot transition from Pause to " + nextGameState); nextGameState = currentGameState; }
                break;
            case GameState.save:
                if (nextGameState == GameState.pause) {OnSaveToPause?.Invoke(); currentGameState = nextGameState; Time.timeScale = 0f; }
                else if (nextGameState == GameState.play) {OnSaveToPlay?.Invoke(); currentGameState = nextGameState; }
                else {Debug.LogError("[GameManager] Cannot transition from Save to " + nextGameState); nextGameState = currentGameState; }
                break;
            default:
                Debug.LogError("[GameManager] Unknown GameState");
                break;
        }
    }

    /// <summary>
    /// OnDestroy method clears the managers
    /// </summary>
    protected override void OnDestroy()
    {
        base.OnDestroy();

        ClearInstantiatedManagers();

        OnPauseToStart -= ClearLoadedLevel;
        OnPlayToStart -= ClearLoadedLevel;
    }

    /// <summary>
    /// QuitGame method quits the game (back to desktop)
    /// </summary>
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    /// <summary>
    /// QuitGameStatic method is a static method that calls QuitGame
    /// </summary>
    public static void QuitGameStatic()
    {
        Instance.QuitGame();
    }
}
