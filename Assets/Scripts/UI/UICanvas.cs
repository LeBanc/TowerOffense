using UnityEngine;

/// <summary>
/// UICanvas is a basic component to add to Unity Canvas component to allow easy Show and Hide methods
/// UICanvas requires the GameObject to have a Canvas component
/// </summary>
[RequireComponent(typeof(Canvas))]
public class UICanvas : MonoBehaviour
{
    // Mandatory canvas component
    protected Canvas canvas;

    public delegate void UICanvasEventHandler();
    public event UICanvasEventHandler OnHide;

    /// <summary>
    /// At Awake, fetches the Canvas
    /// </summary>
    protected virtual void Awake()
    {
        canvas = GetComponent<Canvas>();
        if (canvas == null) Debug.LogError("[UICanvas] Couldn't find mandatory Canvas");
    }

    /// <summary>
    /// IsVisible parameter returns the Canvas visibility
    /// </summary>
    public bool IsVisible
    {
        get { return canvas.enabled; }
    }

    /// <summary>
    /// Show method enables the canvas and sets it above of all other canvas of the same hierarchy
    /// </summary>
    public virtual void Show()
    {
        transform.SetAsLastSibling();
        canvas.enabled = true;
    }

    /// <summary>
    /// Hide method disables the Canvas and sets it under all other canvas of the same hierarchy
    /// </summary>
    public virtual void Hide()
    {
        canvas.enabled = false;
        transform.SetAsFirstSibling();
        OnHide?.Invoke();
    }

    /// <summary>
    /// Display method shows or hides the Canvas depending of the state in parameter
    /// </summary>
    /// <param name="_state">True to show, false to hide (bool)</param>
    public virtual void Display(bool _state)
    {
        if(_state)
        {
            if(!IsVisible) Show();
        }
        else
        {
            if(IsVisible) Hide();
        }
    }
}
