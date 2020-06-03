using UnityEngine;

/// <summary>
/// ColorReturn class is used in ClorPickerCanvas to return a color when clicking on it
/// </summary>
public class ColorReturn : MonoBehaviour
{
    // Color to return
    public Color color;

    // Events
    public delegate void ColorRetrunEventsHandler(Color _c);
    public event ColorRetrunEventsHandler OnPreview;
    public event ColorRetrunEventsHandler OnSelection;

    /// <summary>
    /// ColorPreview calls the OnPreview event
    /// </summary>
    public void ColorPreview()
    {
        OnPreview?.Invoke(color);
    }

    /// <summary>
    /// ColorSelection calls the OnSelection event
    /// </summary>
    public void ColorSelection()
    {
        OnSelection?.Invoke(color);
    }

}
