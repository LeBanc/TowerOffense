﻿using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CursorManager : Singleton<CursorManager>
{
    // Public image for software cursor
    public Image cursor;

    // Public textures for cursor icons
    public Sprite basicCursor;
    public Sprite moveCursor;
    public Sprite notMoveCursor;
    public Sprite attackCursor;
    public Sprite buildCursor;
    public Sprite notBuildCursor;
    public Sprite explosivesCursor;
    public Sprite notExplosivesCursor;

    public static bool mouseControl;

    private bool forceVisibility;
    private Vector2 lastPos;
    private float gamePadSpeed = 500f;

    public delegate void CursorManagerEventHandler();
    public static event CursorManagerEventHandler OnMouseControlChange;
    public static event CursorManagerEventHandler OnGamePadeControlChange;

    // Cursor state defines the state of the cursor (state can have several cursor's icons)
    public enum CursorState
    {
        Basic,
        MoveAttack,
        BuildHQ,
        BuildTurret,
        Explosives
    }

    // Cursor states of the cursor manager
    private CursorState previousState;
    private CursorState currentState;
    private CursorState nextState;

    // Cursor icon defines the used icons
    public enum CursorIcon
    {
        Basic,
        Move,
        NotMove,
        Attack,
        Build,
        NotBuild,
        Explosives,
        NotExplosives
    }

    // Current cursor icon used
    private CursorIcon currentIcon;

    /// <summary>
    /// At Start, init the Cursor states and icon and subscribe to events
    /// </summary>
    private void Start()
    {
        currentState = CursorState.Basic;
        nextState = CursorState.Basic;
        previousState = CursorState.Basic;

        Cursor.visible = false;

        currentIcon = CursorIcon.Basic;
        cursor.sprite = basicCursor;

        mouseControl = true;
        ShowCursor();

        //Events
        GameManager.OnPlayToPause += ForceToBasicState;
        GameManager.OnPauseToPlay += ChangeToPreviousState;
        GameManager.OnLoadToPlay += ShowCursor;
        GameManager.OnStartToLoad += HideCursor;
        GameManager.OnPauseToLoad += HideCursor;

        GameManager.OnPlayToPause += HideCursorOnMenu;
        GameManager.OnPauseToPlay += ShowCursor;

        OnMouseControlChange?.Invoke();

        gamePadSpeed = Screen.width / 1.5f;
    }

    /// <summary>
    /// OnDestroy, unsubscribe from events
    /// </summary>
    protected override void OnDestroy()
    {
        GameManager.OnPlayToPause -= ForceToBasicState;
        GameManager.OnPauseToPlay -= ChangeToPreviousState;
        GameManager.OnLoadToPlay -= ShowCursor;
        GameManager.OnStartToLoad -= HideCursor;
        GameManager.OnPauseToLoad -= HideCursor;

        GameManager.OnPlayToPause -= HideCursorOnMenu;
        GameManager.OnPauseToPlay -= ShowCursor;

        base.OnDestroy();
    }

    /// <summary>
    /// ChangeToBasic method allow to change the cursor to basic state from button onValueChanged
    /// </summary>
    /// <param name="_isOn">isOn parameter of the onValueChanged method calling (change cursor only if true)</param>
    public static void ChangeToBasic(bool _isOn)
    {
        if (_isOn) Instance.ChangeToBasicState();
    }

    /// <summary>
    /// ChangeToMoveAttack method allow to change the cursor to Move/Attack state from button onValueChanged
    /// </summary>
    /// <param name="_isOn">isOn parameter of the onValueChanged method calling (change cursor only if true)</param>
    public static void ChangeToMoveAttack(bool _isOn)
    {
        if(_isOn)
        {
            Instance.nextState = CursorState.MoveAttack;
        }
    }

    /// <summary>
    /// ChangeToBuildHQ method allow to change the cursor to build HQ from button onValueChanged
    /// </summary>
    /// <param name="_isOn">isOn parameter of the onValueChanged method calling (change cursor only if true)</param>
    public static void ChangeToBuildHQ(bool _isOn)
    {
        if (_isOn)
        {
            Instance.nextState = CursorState.BuildHQ;
        }
    }

    /// <summary>
    /// ChangeToBuildTurret method allow to change the cursor to build Turret state from button onValueChanged
    /// </summary>
    /// <param name="_isOn">isOn parameter of the onValueChanged method calling (change cursor only if true)</param>
    public static void ChangeToBuildTurret(bool _isOn)
    {
        if (_isOn)
        {
            Instance.nextState = CursorState.BuildTurret;
        }
    }

    /// <summary>
    /// ChangeToExplosives method allow to change the cursor to explosives state from button onValueChanged
    /// </summary>
    /// <param name="_isOn">isOn parameter of the onValueChanged method calling (change cursor only if true)</param>
    public static void ChangeToExplosives(bool _isOn)
    {
        if (_isOn)
        {
            Instance.nextState = CursorState.Explosives;
        }
    }

    /// <summary>
    /// ChangeToBasicState method changes the cursor state to Basic
    /// This method is used when going to Pause menu
    /// </summary>
    private void ChangeToBasicState()
    {
        nextState = CursorState.Basic;
    }

    /// <summary>
    /// ChangeToPreviousState method changes the cursor state to the previous state
    /// This method is used when going out of the pause menu
    /// </summary>
    private void ChangeToPreviousState()
    {
        nextState = previousState;
    }

    /// <summary>
    /// ForceToBasicState method change the cursor stat to Basic and save the previous state
    /// This method should be used when going in Pause to save the previous state and going back to it when coming back from Pause
    /// </summary>
    private void ForceToBasicState()
    {
        previousState = currentState;
        ChangeToBasicState();
    }

    /// <summary>
    /// On Update, change the state id needed and Raycast the scene to know which cursor icon should be displayed
    /// </summary>
    private void Update()
    {
        // Check to hide hardware cursor if application is focused
        if (Application.isFocused && Cursor.visible) Cursor.visible = false;

        CheckForControllerChange();

        // If the cursor is hidden, no need to change its shape or position
        if (!cursor.enabled) return;

        // Set the current way of handling cursor (mouse or gamepad)
        if (mouseControl)
        {
            CustomInputModule.UpdateCursorPosition(Input.mousePosition);
        }
        else
        {
            CustomInputModule.MoveCursorPosition(new Vector2(Input.GetAxis("Controller X")*gamePadSpeed*Time.deltaTime, Input.GetAxis("Controller Y")*gamePadSpeed*Time.deltaTime));
        }

        // Set the state & shape of the cursor
        // Change the state if needed
        if (nextState != currentState)
        {
            // Default is basic cursor
            SetCursorIcon(CursorIcon.Basic);
            currentState = nextState;
        }

        // If there is no main camera, the Raycast cannot be done, so return here
        if (Camera.main == null) return;

        // Raycast tools
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(CustomInputModule.CursorPos);

        // Change the cursor icon depending of the current state and the object under the mouse
        switch (currentState)
        {
            // Basic state: do nothing, the cursor is already basic
            case CursorState.Basic:
                break;
            // Move attack state : Attack on enemy (tower and enemy soldier), move on terrain and don't move on any other things
            case CursorState.MoveAttack:
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    // Attack icon on enemy if it is active
                    if(hit.collider.TryGetComponent(out Enemy _enemy))
                    {
                        if(_enemy.IsActive())
                        {
                            SetCursorIcon(CursorIcon.Attack);
                        }
                        else
                        {
                            SetCursorIcon(CursorIcon.NotMove);
                        }
                        
                    }
                    // Move icon on terrain if no enemy soldier or turrets on it, else attack or not move icon
                    else if (hit.collider.TryGetComponent(out Terrain _terrain))
                    {
                        // Check if there is an enemy soldier, a turret or a turretBase on the spot
                        bool foundEnemy = false;
                        bool foundTurret = false;
                        foreach (EnemySoldier _enemySoldier in PlayManager.enemyList)
                        {
                            if (GridAdjustment.IsSameOnGrid(_enemySoldier.transform.position, hit.point))
                            {
                                foundEnemy = true;
                            }
                        }
                        foreach (Turret _turret in PlayManager.turretList)
                        {
                            if (GridAdjustment.IsSameOnGrid(_turret.transform.position, hit.point))
                            {
                                foundTurret = true;
                            }
                        }
                        foreach (TurretBase _turretBase in PlayManager.turretBaseList)
                        {
                            if (GridAdjustment.IsSameOnGrid(_turretBase.transform.position, hit.point))
                            {
                                foundTurret = true;
                            }
                        }

                        // If there is an enemy at the same position, set the attack icon
                        if (foundEnemy)
                        {
                            SetCursorIcon(CursorIcon.Attack);
                        }
                        // If there is a turret (or a base) at the same position, set the not move icon
                        else if (foundTurret)
                        {
                            SetCursorIcon(CursorIcon.NotMove);
                        }
                        else // Set the move icon
                        {
                            SetCursorIcon(CursorIcon.Move);
                        }
                    }
                    else // For any other object, not move icon is set
                    {
                        SetCursorIcon(CursorIcon.NotMove);
                    }
                }
                else // If no Physics object is raycast, set the basic icon (over UI)
                {
                    SetCursorIcon(CursorIcon.Basic);
                }
                break;
            //BuildHQ state : build over HQCandidate, not build over other Physics object and basic over "nothing"
            case CursorState.BuildHQ:
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    if(hit.collider.TryGetComponent(out HQCandidate _hqCandidate))
                    {
                        SetCursorIcon(CursorIcon.Build);
                    }
                    else
                    {
                        SetCursorIcon(CursorIcon.NotBuild);
                    }
                }
                else
                {
                    SetCursorIcon(CursorIcon.Basic);
                }
                break;
            //BuildTurret state : build over empty terrain, not build over other Physics object and basic over "nothing"
            case CursorState.BuildTurret:
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    if (hit.collider.TryGetComponent(out Terrain _terrain))
                    {
                        bool foundEnemy = false;
                        foreach (EnemySoldier _enemy in PlayManager.enemyList)
                        {
                            if (GridAdjustment.IsSameOnGrid(_enemy.transform.position, hit.point))
                            {
                                foundEnemy = true;
                            }
                        }
                        if(foundEnemy)
                        {
                            SetCursorIcon(CursorIcon.NotBuild);
                        }
                        else
                        {
                            SetCursorIcon(CursorIcon.Build);
                        }                        
                    }
                    else
                    {
                        SetCursorIcon(CursorIcon.NotBuild);
                    }
                }
                else
                {
                    SetCursorIcon(CursorIcon.Basic);
                }
                break;
            //Explosives state : explosives over active tower, not explosives otherwise and basic over "nothing"
            case CursorState.Explosives:
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    if (hit.collider.TryGetComponent(out Tower _tower))
                    {
                        if(_tower.IsActive())
                        {
                            SetCursorIcon(CursorIcon.Explosives);
                        }
                        else
                        {
                            SetCursorIcon(CursorIcon.NotExplosives);
                        }                        
                    }
                    else
                    {
                        SetCursorIcon(CursorIcon.NotExplosives);
                    }
                }
                else
                {
                    SetCursorIcon(CursorIcon.Basic);
                }
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// SetCursorIcon method changes the cursor icon depending on the current CursorIcon and the requested CursorIcon
    /// </summary>
    /// <param name="_icon">Requested CursorIcon</param>
    private void SetCursorIcon(CursorIcon _icon)
    {
        // Change only if needed
        if(_icon != currentIcon)
        {
            switch (_icon)
            {
                case CursorIcon.Basic:
                    cursor.sprite = basicCursor;
                    break;
                case CursorIcon.Move:
                    cursor.sprite = moveCursor;
                    break;
                case CursorIcon.NotMove:
                    cursor.sprite = notMoveCursor;
                    break;
                case CursorIcon.Attack:
                    cursor.sprite = attackCursor;
                    break;
                case CursorIcon.Build:
                    cursor.sprite = buildCursor;
                    break;
                case CursorIcon.NotBuild:
                    cursor.sprite = notBuildCursor;
                    break;
                case CursorIcon.Explosives:
                    cursor.sprite = explosivesCursor;
                    break;
                case CursorIcon.NotExplosives:
                    cursor.sprite = notExplosivesCursor;
                    break;
            }
            currentIcon = _icon;
        }
    }

    /// <summary>
    /// HideCursor method hides the cursor (used by subscribing to GameManager events)
    /// </summary>
    private void HideCursor()
    {
        cursor.enabled = false;
        CustomInputModule.UpdateCursorPosition(Vector2.zero);
    }

    private void HideCursorOnMenu()
    {
        if(!mouseControl)
        {
            lastPos = CustomInputModule.CursorPos;
            HideCursor();
        }
    }

    /// <summary>
    /// ShowCursur method shows the cursor (used by subscribing to GameManager events)
    /// </summary>
    private void ShowCursor()
    {
        if (mouseControl)
        {
            cursor.enabled = true;
            CustomInputModule.ResetMinMaxCursorPosition();
        }
        else if(forceVisibility)
        {
            cursor.enabled = true;
            CustomInputModule.UpdateCursorPosition(lastPos);
            lastPos = Vector2.zero;
        }
    }

    /// <summary>
    /// ShowCursorForAction method shows the cursor if controller is gamepad
    /// </summary>
    public static void ShowCursorForAction(Vector2 _position, Vector2 _min, Vector2 _max)
    {
        if (!mouseControl)
        {
            CustomInputModule.SetMinMaxCursorPosition(_min, _max);
            if(!Instance.cursor.enabled)CustomInputModule.UpdateCursorPosition(_position);
            Instance.cursor.enabled = true;
            Instance.forceVisibility = true;
        }
    }

    /// <summary>
    /// HideCursorAfterAction method hides the cursor if controller is gamepad
    /// </summary>
    public static void HideCursorAfterAction()
    {
        if (!mouseControl && Instance.forceVisibility)
        {
            Instance.cursor.enabled = false;
            Instance.forceVisibility = false;
            CustomInputModule.ResetMinMaxCursorPosition();
            CustomInputModule.UpdateCursorPosition(Vector2.zero);
        }
    }

    /// <summary>
    /// CheckForControllerChange method checks if gamePad or mouse/keyboard is used to change the current controller
    /// </summary>
    private void CheckForControllerChange()
    {
        bool prevMouseControl = mouseControl;
        
        // Mouse movement
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0 || Input.mouseScrollDelta.y != 0)
        {
            mouseControl = true;
        }
        // Mouse buttons
        if(Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
        {
            mouseControl = true;
        }

        // GamePad axes movement
        if(Input.GetAxis("Controller X") != 0 || Input.GetAxis("Controller Y") != 0 ||
           Input.GetAxis("Controller_RightStick Horizontal") != 0 || Input.GetAxis("Controller_RightStick Vertical") != 0 ||
           Input.GetAxis("Controller_DPad Horizontal") != 0 || Input.GetAxis("Controller_DPad Vertical") != 0 ||
           Input.GetAxis("Controller_Left Trigger") != 0 || Input.GetAxis("Controller_Right Trigger") != 0)
        {
            mouseControl = false;
        }

        // Any button
        if(Input.anyKeyDown)
        {
            // GamePad button
            if(Input.GetKeyDown(KeyCode.JoystickButton0) || Input.GetKeyDown(KeyCode.JoystickButton1) || Input.GetKeyDown(KeyCode.JoystickButton2) ||
                Input.GetKeyDown(KeyCode.JoystickButton3) || Input.GetKeyDown(KeyCode.JoystickButton4) || Input.GetKeyDown(KeyCode.JoystickButton5) ||
                Input.GetKeyDown(KeyCode.JoystickButton6) || Input.GetKeyDown(KeyCode.JoystickButton7) || Input.GetKeyDown(KeyCode.JoystickButton8) ||
                Input.GetKeyDown(KeyCode.JoystickButton9))
            {
                mouseControl = false;
            }
            else // Keyboard button
            {
                mouseControl = true;
            }
        }

        if (mouseControl && !prevMouseControl)
        {
            OnMouseControlChange?.Invoke();
            ShowCursor();
        }
        if (!mouseControl && prevMouseControl)
        {
            OnGamePadeControlChange?.Invoke();
            if(!forceVisibility)
            {
                HideCursor();
                if (EventSystem.current.currentSelectedGameObject == null && UIManager.lastSelected != null) EventSystem.current.SetSelectedGameObject(UIManager.lastSelected.gameObject);
            }            
        }
    }
}
