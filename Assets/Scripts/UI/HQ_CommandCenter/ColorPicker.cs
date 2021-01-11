using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ColorPicker uses ColorReturn events to preview and select a color
/// "Preview" updates a "Preview" image
/// "Select" updates a "Select" image
/// </summary>
public class ColorPicker : MonoBehaviour
{
    // Both select and preview images
    public Image preview;
    public Image selected;

    // The list of all ColorReturn elements to listen their events
    public List<ColorReturn> colorReturns;

    /// <summary>
    /// At start, link the UpdateColor and SelectColor methods to the events of each ColorReturn
    /// </summary>
    private void Start()
    {
        if (colorReturns.Count > 0)
        {
            foreach(ColorReturn _c in colorReturns)
            {
                _c.OnPreview += UpdateColor;
                _c.OnSelection += SelectColor;
            }
        }
    }

    /// <summary>
    /// Setup is used to initialize the "Select" image color
    /// </summary>
    /// <param name="_color">Color to set the "Select" image to</param>
    public void Setup(Color _color)
    {
        selected.color = _color;
    }

    /// <summary>
    /// UpdateColor methods updates the "Preview" image color
    /// </summary>
    /// <param name="_color">Color to set the "Preview" image to</param>
    private void UpdateColor(Color _color)
    {
        preview.color = _color;
    }

    /// <summary>
    /// SelectColor methods updates the "Select" image color
    /// </summary>
    /// <param name="_color">Color to set the "Select" image to</param>
    private void SelectColor(Color _color)
    {
        selected.color = _color;
    }

    /// <summary>
    /// GetColor returns the current color of the "Select" image
    /// </summary>
    /// <returns></returns>
    public Color GetColor()
    {
        return selected.color;
    }

    /// <summary>
    /// OnDestroy clears all links to the ColorReturn events
    /// </summary>
    private void OnDestroy()
    {
        if (colorReturns.Count > 0)
        {
            foreach (ColorReturn _c in colorReturns)
            {
                _c.OnPreview -= UpdateColor;
                _c.OnSelection -= SelectColor;
            }
        }
    }

    /// <summary>
    /// ColorPickerUpdate is the Update method of ColorPicker. It casts a ray at mouse position and activate methods if it is a ColorReturn
    /// </summary>
    public void ColorPickerUpdate()
    {
        // Cast a ray straight down at mouse position
        Vector2 _mousePos = Input.mousePosition;
        RaycastHit2D hit = Physics2D.Raycast(_mousePos, Vector2.zero);
        // If it hits something
        if (hit.collider != null)
        {
            // If the hit object has a ColorReturn element
            if (hit.collider.TryGetComponent<ColorReturn>(out ColorReturn _colorReturn))
            {
                // Activate the Color Preview (update of preview image via event)
                _colorReturn.ColorPreview();
                // And if the left mouse button is clicked, activate the Color Selection (update of select image via event)
                if (Input.GetMouseButtonDown(0))
                {
                    _colorReturn.ColorSelection();
                }
            }
        }
    }
}
