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
    public HQFacilities facilities;

    /// <summary>
    /// At Start, initialize the Tab bar to display the ComCenter
    /// Subscribe to events to update UI navigation
    /// </summary>
    private void Start()
    {
        comCenter.squad1Header.OnSelection += SetComCenterTabDownNav;
        comCenter.squad2Header.OnSelection += SetComCenterTabDownNav;
        comCenter.squad3Header.OnSelection += SetComCenterTabDownNav;
        comCenter.squad4Header.OnSelection += SetComCenterTabDownNav;
    }

    /// <summary>
    /// OnDestroy, unsubscribe from events
    /// </summary>
    private void OnDestroy()
    {
        comCenter.squad1Header.OnSelection -= SetComCenterTabDownNav;
        comCenter.squad2Header.OnSelection -= SetComCenterTabDownNav;
        comCenter.squad3Header.OnSelection -= SetComCenterTabDownNav;
        comCenter.squad4Header.OnSelection -= SetComCenterTabDownNav;
    }

    /// <summary>
    /// Refresh method refresh the canvases displays
    /// </summary>
    public void Refresh()
    {
        ShowComCenter();
        ShowBarracks();
        ShowFacilities();
    }

    /// <summary>
    /// HideAll method hides all the subcanvas
    /// </summary>
    public void HideAll()
    {
        comCenter.Hide();
        barracks.Hide();
        facilities.Hide();
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

    /// <summary>
    /// ShowFacilities method displays or hides the Facilities Canvas depending on the Facilities toggle state
    /// </summary>
    public void ShowFacilities()
    {
        if(facilitiesTab.isOn)
        {
            facilities.Show();
        }
        else
        {
            facilities.Hide();
        }
    }

    /// <summary>
    /// SetComCenterTabDownNav method updates the UI Navigation of the ComCenter tab to target the selected squad's Squad Header
    /// Not the best way to do it on HQTab but it is a way that works ...
    /// </summary>
    /// <param name="_header">SquadHeader to target</param>
    private void SetComCenterTabDownNav(HQSquadHeader _header)
    {
        Navigation _nav = comCenterTab.navigation;
        _nav.selectOnDown = _header.select;
        comCenterTab.navigation = _nav;
    }

}
