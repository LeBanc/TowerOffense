using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

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

    private void ClearInstantiatedManagers()
    {
        foreach (var go in _instanciatedManagers)
        {
            Destroy(go);
        }
        _instanciatedManagers.Clear();
    }

    private void AddManager(GameObject manager)
    {
        _instanciatedManagers.Add(manager);
    }

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

    public void LoadLevel(string sceneName)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        _loadOperations.Add(ao);
    }

    public void UnloadLevel(string sceneName)
    {
        AsyncOperation ao = SceneManager.UnloadSceneAsync(sceneName);
        _unloadOperations.Add(ao);
    }

    public void StartNewGame()
    {
        if (!SceneManager.GetSceneByName("Test").IsValid() && !SceneManager.GetSceneByName("EmptyScene").IsValid()) LoadLevel("Test");
        Instance.StartCoroutine(LoadingSequence(0));
    }

    public void ContinueGame()
    {
        if (!SceneManager.GetSceneByName("Test").IsValid() && !SceneManager.GetSceneByName("EmptyScene").IsValid()) LoadLevel("EmptyScene");
        Instance.StartCoroutine(LoadingSequence(1));
    }

    public static void LoadGame(string _fileName)
    {
        if (!SceneManager.GetSceneByName("Test").IsValid() && !SceneManager.GetSceneByName("EmptyScene").IsValid()) Instance.LoadLevel("EmptyScene");
        Instance.StartCoroutine(Instance.LoadingSequence(2,_fileName));
    }

    private IEnumerator LoadingSequence(int _loadingType, string _fileName = "")
    {
        float startTime = Time.time;

        ChangeGameStateRequest(GameState.load);
        while(_loadOperations[_loadOperations.Count-1].progress < 1f)
        {
            yield return null;
        }
        PlayManager.LoadFromEmptyScene();
        if(_loadingType == 0)
        {
            PlayManager.LoadFromScene();
        }
        else if(_loadingType == 1)
        {
            DataSave.LoadAutoSavedGame();
            PlayManager.InitAfterLoad();
        }
        else
        {
            DataSave.LoadSavedGame(_fileName);
            PlayManager.InitAfterLoad();
        }

        // Small pause if the loading take less than 2 seconds, just to see the loading screen
        while(Time.time < startTime +2f)
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
                if (nextGameState == GameState.start) {OnPauseToStart?.Invoke(); currentGameState = nextGameState; }
                else if (nextGameState == GameState.play) {OnPauseToPlay?.Invoke(); currentGameState = nextGameState; }
                else if (nextGameState == GameState.save) {OnPauseToSave?.Invoke(); currentGameState = nextGameState; }
                else if (nextGameState == GameState.load) { OnPauseToLoad?.Invoke(); currentGameState = nextGameState; }
                else {Debug.LogError("[GameManager] Cannot transition from Pause to " + nextGameState); nextGameState = currentGameState; }
                break;
            case GameState.save:
                if (nextGameState == GameState.pause) {OnSaveToPause?.Invoke(); currentGameState = nextGameState; }
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
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    public static void QuitGameStatic()
    {
        Instance.QuitGame();
    }
}
