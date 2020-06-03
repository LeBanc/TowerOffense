using UnityEngine;

/// <summary>
/// PlayUI is the higher class UI of the Play GameState
/// </summary>
public class PlayUI : MonoBehaviour
{
    // Public Canvas composing the Play UI
    public HQCanvas hqCanvas;
    public CityCanvas cityCanvas;

    /// <summary>
    /// At Awake, hides all the canvas
    /// </summary>
    private void Awake()
    {
        ShowCityCanvas(false);
        ShowHQCanvas(false);
    }

    /// <summary>
    /// ShowHQCanvas method displays the HQ UI
    /// </summary>
    /// <param name="_show">Enabled boolean</param>
    public void ShowHQCanvas(bool _show)
    {
        if (_show)
        {
            // Show and Upadte the Canvas
            hqCanvas.Show();
            UpdateHQCanvas();
        }
        else
        {
            // Hide the Canvas
            hqCanvas.Hide();
        }
    }

    /// <summary>
    /// UpdateHQCanvas method draws the HS canvas
    /// </summary>
    public void UpdateHQCanvas()
    {
        // Update this method when HQ Canvas has more than CommandCenter in it
        hqCanvas.commandCenter.UpdateSquadHeaders();
    }

    /// <summary>
    /// ShowCityCanvas method displays the City UI (attack)
    /// </summary>
    /// <param name="_show">Enabled boolean</param>
    public void ShowCityCanvas(bool _show)
    {
        if (_show)
        {
            // Show the City canvas
            cityCanvas.Show();
        }
        else
        {
            // Hide the City canvas
            cityCanvas.Hide();
        }
    }

    /// <summary>
    /// ResetCityCanvas clears the SquadActionPanels in the City canvas
    /// </summary>
    public void ResetCityCanvas()
    {
        cityCanvas.Reset();
    }



}
