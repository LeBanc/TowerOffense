using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// CustomInputModule class defines a derivate StandaloneInputModule for software cursor
/// It allows to move the cursor with mouse and gamePad controller
/// </summary>
public class CustomInputModule : StandaloneInputModule
{
    private static Vector2 m_cursorPos;

    private static Vector2 m_min;
    private static Vector2 m_max;
    private Vector2 screenSize;

    /// <summary>
    /// UpdateCursorPosition method moves the cursor to the desired location
    /// </summary>
    /// <param name="a_pos">Desired position of the cursor (Vector2)</param>
    public static void UpdateCursorPosition(Vector2 a_pos)
    {
        m_cursorPos = Vector2.Min(Vector2.Max(a_pos, m_min), m_max);
    }

    /// <summary>
    /// MoveCursorPosition method moves of the desired delta-position from the current position
    /// </summary>
    /// <param name="a_pos">Desired delat-position (Vector2)</param>
    public static void MoveCursorPosition(Vector2 a_pos)
    {
        m_cursorPos += a_pos;
        m_cursorPos = Vector2.Min(Vector2.Max(m_cursorPos, m_min), m_max);
    }

    /// <summary>
    /// SetMinMaxCursorPosition method defines a square (rect) with min and max positions that the cursor cannot exit
    /// </summary>
    /// <param name="_min">Min screen position (Vect2)</param>
    /// <param name="_max">Max screen positin (Vector2)</param>
    public static void SetMinMaxCursorPosition(Vector2 _min, Vector2 _max)
    {
        m_min = _min;
        m_max = _max;
    }

    /// <summary>
    /// ResetMinMaxCursorPosition method reset the min and max allowed cursor position to the whole screen
    /// </summary>
    public static void ResetMinMaxCursorPosition()
    {
        m_min = Vector2.zero;
        m_max = new Vector2(Screen.width, Screen.height);
    }

    private void ScreenSizeChange()
    {
        m_min = new Vector2(m_min.x * Screen.width / screenSize.x, m_min.y * Screen.height / screenSize.y);
        m_max = new Vector2(m_max.x * Screen.width / screenSize.x, m_max.y * Screen.height / screenSize.y);
        screenSize = new Vector2(Screen.width, Screen.height);
    }

    private void Update()
    {
        if (screenSize.x != Screen.width || screenSize.y != Screen.height) ScreenSizeChange();
    }

    /// <summary>
    /// Get the current cursor position
    /// </summary>
    public static Vector2 CursorPos
    {
        get { return m_cursorPos; }
    }

    /// <summary>
    /// Get the current screen ratio
    /// </summary>
    public static Vector2 ScreenRatio
    {
        get { return new Vector2(Screen.width / 1306f, Screen.height / 735f); }
    }

    /// <summary>
    /// At awake, reset the cursor min & max allowed positions
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        screenSize = new Vector2(Screen.width, Screen.height);
        ResetMinMaxCursorPosition();
    }

    // This is the real function we want, the two commented out lines (Input.mousePosition) are replaced with m_cursorPos (our fake mouse position, set with the public function, UpdateCursorPosition)
    private readonly MouseState m_MouseState = new MouseState();
    protected override MouseState GetMousePointerEventData(int id =0)
    {
        MouseState m = new MouseState();

        // Populate the left button...
        PointerEventData leftData;
        var created = GetPointerData(kMouseLeftId, out leftData, true);

        leftData.Reset();

        if (created)
            leftData.position = m_cursorPos;
        //leftData.position = Input.mousePosition;

        //Vector2 pos = Input.mousePosition;
        Vector2 pos = m_cursorPos;
        leftData.delta = pos - leftData.position;
        leftData.position = pos;
        leftData.scrollDelta = Input.mouseScrollDelta;
        leftData.button = PointerEventData.InputButton.Left;
        eventSystem.RaycastAll(leftData, m_RaycastResultCache);
        var raycast = FindFirstRaycast(m_RaycastResultCache);
        leftData.pointerCurrentRaycast = raycast;
        m_RaycastResultCache.Clear();

        // copy the apropriate data into right and middle slots
        PointerEventData rightData;
        GetPointerData(kMouseRightId, out rightData, true);
        CopyFromTo(leftData, rightData);
        rightData.button = PointerEventData.InputButton.Right;

        PointerEventData middleData;
        GetPointerData(kMouseMiddleId, out middleData, true);
        CopyFromTo(leftData, middleData);
        middleData.button = PointerEventData.InputButton.Middle;

        m_MouseState.SetButtonState(PointerEventData.InputButton.Left, StateForMouseButton(0), leftData);
        m_MouseState.SetButtonState(PointerEventData.InputButton.Right, StateForMouseButton(1), rightData);
        m_MouseState.SetButtonState(PointerEventData.InputButton.Middle, StateForMouseButton(2), middleData);

        return m_MouseState;
    }
}
