﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    #region GameState
    public enum GameState
    {
        start,
        load,
        play,
        pause,
        save
    }

    private static GameState currentGameState;
    private static GameState previousGameState;
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

    public static event GameStateEventHandler OnStartToLoad;
    public static event GameStateEventHandler OnLoadToPlay;
    public static event GameStateEventHandler OnPlayToStart;
    public static event GameStateEventHandler OnPlayToPause;
    public static event GameStateEventHandler OnPlayToLoad;
    public static event GameStateEventHandler OnPauseToSave;
    public static event GameStateEventHandler OnPauseToPlay;
    public static event GameStateEventHandler OnPauseToLoad;
    public static event GameStateEventHandler OnPauseToStart;
    public static event GameStateEventHandler OnSaveToPause;

    public static event GameStateEventHandler StartUpdate;
    public static event GameStateEventHandler LoadUpdate;
    public static event GameStateEventHandler PlayUpdate;
    public static event GameStateEventHandler PauseUpdate;
    public static event GameStateEventHandler SaveUpdate;

    #endregion

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
    #endregion

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
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        currentGameState = GameState.start;
        _instanciatedManagers = new List<GameObject>();
        InstantiateManagers();
    }

    // Update is called once per frame
    void Update()
    {
        // Change state if needed
        if (currentGameState != nextGameState) ChangeGameState();

        // Basic state machine
        switch (GameManager.currentGameState)
        {
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

    void ChangeGameState()
    {
        switch (GameManager.currentGameState)
        {
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
                else if (nextGameState == GameState.start) {OnPlayToStart?.Invoke(); currentGameState = nextGameState; }
                else if (nextGameState == GameState.load) { OnPlayToLoad?.Invoke(); currentGameState = nextGameState; }
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
                else {Debug.LogError("[GameManager] Cannot transition from Save to " + nextGameState); nextGameState = currentGameState; }
                break;
            default:
                Debug.LogError("[GameManager] Unknown GameState");
                break;
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        ClearInstantiatedManagers();
    }
}
