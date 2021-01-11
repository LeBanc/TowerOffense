using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// HQTabToggle class helps switching between Canvas when clicking on HQ Tabs
/// </summary>
public class HQTabToggle : MonoBehaviour
{
    // All Toggle elements in the tab bar
    public Toggle comCenterTab;
    public Toggle barracksTab;
    public Toggle intelligenceTab;
    public Toggle facilitiesTab;
    public Toggle memorialTab;

    // Specific canvas to display
    public HQCommandCenter comCenter;
    public HQBarracks barracks;

    /// <summary>
    /// At Start, initialize the Tab bar to display the ComCenter
    /// </summary>
    private void Start()
    {
        //comCenterTab.Select();
        //comCenterTab.isOn = true;
        //Refresh();
    }

    /// <summary>
    /// Refresh method refresh the canvases displays
    /// </summary>
    public void Refresh()
    {
        ShowComCenter();
        ShowBarracks();
    }

    /// <summary>
    /// HideAll method hides all the subcanvas
    /// </summary>
    public void HideAll()
    {
        comCenter.Hide();
        barracks.Hide();
    }

    /// <summary>
    /// ShowComCenter method displays or hides the ComCenter Canvas depending on the ComCenter toggle state
    /// </summary>
    public void ShowComCenter()
    {
        if (comCenterTab.isOn)
        {
            comCenter.Show();
        }
        else
        {
            comCenter.Hide();
        }
    }

    /// <summary>
    /// ShowBarracks method displays or hides the Barracks Canvas depending on the Barracks toggle state
    /// </summary>
    public void ShowBarracks()
    {
        if (barracksTab.isOn)
        {
            barracks.Show();
        }
        else
        {
            barracks.Hide();
        }
    }

}
