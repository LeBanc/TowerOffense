using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// HQCanvas is the class used to manage the overall Menu of HQ management
/// </summary>
public class HQCanvas : UICanvas
{
    // Text label to display 'Day' value
    public Text day;
    // Text label to display 'Coins' value
    public Text coins;
    // TabToggle
    public HQTabToggle tabs;
    // Default selected component
    public Selectable defaultSelected;

    /// <summary>
    /// On Awake, get the Canvas component and subscribe to events
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        PlayManager.OnCoinsUpdate += UpdateCoins;
        PlayManager.OnLoadSquadsOnNewDay += Hide;
        PlayManager.OnEndDay += Show;
        PlayManager.OnEndDay += Init;
        PlayManager.OnLoadGame += Init;
    }

    /// <summary>
    /// OnDestroy, unsubscribe from events
    /// </summary>
    private void OnDestroy()
    {
        PlayManager.OnCoinsUpdate -= UpdateCoins;
        PlayManager.OnLoadSquadsOnNewDay -= Hide;
        PlayManager.OnEndDay -= Show;
        PlayManager.OnEndDay -= Init;
        PlayManager.OnLoadGame -= Init;
    }

    /// <summary>
    /// Hide method hides the HQCanvas
    /// </summary>
    public override void Hide()
    {
        tabs.HideAll();
        base.Hide();
    }

    /// <summary>
    /// Init refresh the canvas data (upadte Day and Coins texts) and select the default selectable
    /// </summary>
    public void Init()
    {
        tabs.comCenterTab.isOn = true;
        tabs.Refresh();
        UpdateDay();
        UpdateCoins();
        defaultSelected.Select();
    }

    /// <summary>
    /// UpdateDay method updates the "Day" text with the current day value
    /// </summary>
    public void UpdateDay()
    {
        day.text = PlayManager.day.ToString();
    }

    /// <summary>
    /// UpdateCoins method updates the "Coins" text with the current coins value
    /// </summary>
    public void UpdateCoins()
    {
        coins.text = PlayManager.coins.ToString();
    }
}
