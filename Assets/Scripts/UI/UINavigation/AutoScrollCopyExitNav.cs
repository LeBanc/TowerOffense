using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// AutoScrollCopyExitNav class is used to copy the Navigation of an AutoScroll exits (Selectable) to the associated Selectable
/// </summary>

// AutoScroll requires the GameObject to have a Selectable component
[RequireComponent(typeof(Selectable))]
public class AutoScrollCopyExitNav : MonoBehaviour
{
    // Autoscroll from which copying the navigation
    public AutoScroll autoscroll;
    // booleans to set which exits is copied
    public bool copyUpExit;
    public bool copyDownExit;
    public bool copyLeftExit;
    public bool copyRightExit;

    // Associated selectable (on which the navigation will be paste)
    private Selectable selectable;

    /// <summary>
    /// On Start, fetches the Selectable and subscribe to events
    /// </summary>
    private void Start()
    {
        selectable = GetComponent<Selectable>();

        // If no selectable or no autoscroll, do nothing!
        if (selectable == null || autoscroll == null) return;

        // if CopyUp and the exit exist: Copy the Up Exit navigation each time it is changed
        if (copyUpExit && autoscroll.upExit != null) autoscroll.OnUpExitNavigationSet += SetNavFromUpExit;
        // if CopyDown and the exit exist: Copy the Down Exit navigation each time it is changed
        if (copyDownExit && autoscroll.downExit!=null) autoscroll.OnDownExitNavigationSet += SetNavFromDownExit;
        // if CopyLeft and the exit exist: Copy the Left Exit navigation each time it is changed
        if (copyLeftExit && autoscroll.leftExit != null) autoscroll.OnLeftExitNavigationSet += SetNavFromLeftExit;
        // if CopyRight and the exit exist: Copy the Right Exit navigation each time it is changed
        if (copyRightExit && autoscroll.rightExit != null) autoscroll.OnRightExitNavigationSet += SetNavFromRightExit;
    }

    /// <summary>
    /// OnDestroy, unsubscribe from events
    /// </summary>
    private void OnDestroy()
    {
        autoscroll.OnUpExitNavigationSet -= SetNavFromUpExit;
        autoscroll.OnDownExitNavigationSet -= SetNavFromDownExit;
        autoscroll.OnLeftExitNavigationSet -= SetNavFromLeftExit;
        autoscroll.OnRightExitNavigationSet -= SetNavFromRightExit;
    }

    /// <summary>
    /// SetNavFromUpExit method sets the Down navigation target of the Selectable
    /// </summary>
    /// <param name="_selectable">Selectable to select on down navigation</param>
    private void SetNavFromUpExit(Selectable _selectable)
    {
            Navigation _nav = selectable.navigation;
            _nav.selectOnDown = _selectable;
            selectable.navigation = _nav;
    }

    /// <summary>
    /// SetNavFromDownExit method sets the Up navigation target of the Selectable
    /// </summary>
    /// <param name="_selectable">Selectable to select on up navigation</param>
    private void SetNavFromDownExit(Selectable _selectable)
    {
            Navigation _nav = selectable.navigation;
            _nav.selectOnUp = _selectable;
            selectable.navigation = _nav;
    }

    /// <summary>
    /// SetNavFromLeftExit method sets the Right navigation target of the Selectable
    /// </summary>
    /// <param name="_selectable">Selectable to select on right navigation</param>
    private void SetNavFromLeftExit(Selectable _selectable)
    {
            Navigation _nav = selectable.navigation;
            _nav.selectOnRight = _selectable;
            selectable.navigation = _nav;
    }

    /// <summary>
    /// SetNavFromRightExit method sets the Left navigation target of the Selectable
    /// </summary>
    /// <param name="_selectable">Selectable to select on left navigation</param>
    private void SetNavFromRightExit(Selectable _selectable)
    {
            Navigation _nav = selectable.navigation;
            _nav.selectOnLeft = _selectable;
            selectable.navigation = _nav;
    }
}
