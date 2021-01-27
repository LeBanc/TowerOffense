using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UICanvas is a basic component to add to Unity Canvas component to allow easy Show and Hide methods
/// UICanvas requires the GameObject to have a Canvas component
/// </summary>
[RequireComponent(typeof(Canvas))]
public class UICanvas : MonoBehaviour
{
    public List<GameObject> webGLObjectsToHide = new List<GameObject>();

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

#if UNITY_WEBGL
        // Remove the selected objects (menu buttons for continue/save/load, all things not available in WEBGL)
        foreach (GameObject _go in webGLObjectsToHide)
        {
            // Set the UI navigation of linked selectables
            if(_go.TryGetComponent(out Selectable _sel))
            {
                Navigation _nav = _sel.navigation;
                if(_nav.selectOnUp != null)
                {
                    Navigation _navUp = _nav.selectOnUp.navigation;
                    _navUp.selectOnDown = _nav.selectOnDown;
                    _nav.selectOnUp.navigation = _navUp;
                }
                if (_nav.selectOnDown != null)
                {
                    Navigation _navDown = _nav.selectOnDown.navigation;
                    _navDown.selectOnUp = _nav.selectOnUp;
                    _nav.selectOnDown.navigation = _navDown;
                }
                if (_nav.selectOnLeft != null)
                {
                    Navigation _navLeft = _nav.selectOnLeft.navigation;
                    _navLeft.selectOnRight = _nav.selectOnRight;
                    _nav.selectOnLeft.navigation = _navLeft;
                }
                if (_nav.selectOnRight != null)
                {
                    Navigation _navRight = _nav.selectOnRight.navigation;
                    _navRight.selectOnLeft = _nav.selectOnLeft;
                    _nav.selectOnRight.navigation = _navRight;
                }
            }
            // Hide the object
            _go.SetActive(false);
        }
#endif
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
