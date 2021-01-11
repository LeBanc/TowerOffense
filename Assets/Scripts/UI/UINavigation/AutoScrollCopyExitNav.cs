using UnityEngine;
using UnityEngine.UI;

// AutoScroll requires the GameObject to have a Selectable component
[RequireComponent(typeof(Selectable))]
public class AutoScrollCopyExitNav : MonoBehaviour
{
    public AutoScroll autoscroll;
    public bool copyUpExit;
    public bool copyDownExit;
    public bool copyLeftExit;
    public bool copyRightExit;

    private Selectable selectable;

    private void Start()
    {
        selectable = GetComponent<Selectable>();

        // If no selectable or no autoscroll, do nothing!
        if (selectable == null || autoscroll == null) return;

        if (copyUpExit && autoscroll.upExit != null) autoscroll.OnUpExitNavigationSet += SetNavFromUpExit;
        if (copyDownExit && autoscroll.downExit!=null) autoscroll.OnDownExitNavigationSet += SetNavFromDownExit;
        if (copyLeftExit && autoscroll.leftExit != null) autoscroll.OnLeftExitNavigationSet += SetNavFromLeftExit;
        if (copyRightExit && autoscroll.rightExit != null) autoscroll.OnRightExitNavigationSet += SetNavFromRightExit;
    }

    private void OnDestroy()
    {
        autoscroll.OnUpExitNavigationSet -= SetNavFromUpExit;
        autoscroll.OnDownExitNavigationSet -= SetNavFromDownExit;
        autoscroll.OnLeftExitNavigationSet -= SetNavFromLeftExit;
        autoscroll.OnRightExitNavigationSet -= SetNavFromRightExit;
    }

    private void SetNavFromUpExit(Selectable _selectable)
    {
            Navigation _nav = selectable.navigation;
            _nav.selectOnDown = _selectable;
            selectable.navigation = _nav;
    }

    private void SetNavFromDownExit(Selectable _selectable)
    {
            Navigation _nav = selectable.navigation;
            _nav.selectOnUp = _selectable;
            selectable.navigation = _nav;
    }

    private void SetNavFromLeftExit(Selectable _selectable)
    {
            Navigation _nav = selectable.navigation;
            _nav.selectOnRight = _selectable;
            selectable.navigation = _nav;
    }

    private void SetNavFromRightExit(Selectable _selectable)
    {
            Navigation _nav = selectable.navigation;
            _nav.selectOnLeft = _selectable;
            selectable.navigation = _nav;
    }
}
